using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DrillSensorsEmulator.Database;

public partial class RitnavSystemForDrillMachinesContext : DbContext
{
    public RitnavSystemForDrillMachinesContext()
    {
    }

    public RitnavSystemForDrillMachinesContext(DbContextOptions<RitnavSystemForDrillMachinesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CoordinatesDrillPolygon> CoordinatesDrillPolygons { get; set; }

    public virtual DbSet<DrillHole> DrillHoles { get; set; }

    public virtual DbSet<DrillMachine> DrillMachines { get; set; }

    public virtual DbSet<DrillPolygon> DrillPolygons { get; set; }

    public virtual DbSet<PositionsDrillMachine> PositionsDrillMachines { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Server=109.174.29.40:5432; DataBase=RITNavSystemForDrillMachines;Integrated Security=false; User Id=Ivan;password=12345678");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CoordinatesDrillPolygon>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("CoordinatesDrillPolygon_pkey");

            entity.ToTable("CoordinatesDrillPolygon");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.IddrillPolygon).HasColumnName("IDDrillPolygon");

            entity.HasOne(d => d.IddrillPolygonNavigation).WithMany(p => p.CoordinatesDrillPolygons)
                .HasForeignKey(d => d.IddrillPolygon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("DrillPolygon_Coordinates");
        });

        modelBuilder.Entity<DrillHole>(entity =>
        {
            entity.HasKey(e => e.IddrillHole).HasName("Drillhole_pkey");

            entity.ToTable("DrillHole");

            entity.Property(e => e.IddrillHole)
                .HasDefaultValueSql("nextval('\"Drillhole_IDDrillHole_seq\"'::regclass)")
                .HasColumnName("IDDrillHole");
            entity.Property(e => e.DesignatingTag).HasMaxLength(50);
            entity.Property(e => e.IddrillPolygon).HasColumnName("IDDrillPolygon");

            entity.HasOne(d => d.IddrillPolygonNavigation).WithMany(p => p.DrillHoles)
                .HasForeignKey(d => d.IddrillPolygon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("DrillPolygon_DrillHole");
        });

        modelBuilder.Entity<DrillMachine>(entity =>
        {
            entity.HasKey(e => e.IddrillingMachine).HasName("DrillingMachines_pkey");

            entity.ToTable("DrillMachine");

            entity.Property(e => e.IddrillingMachine)
                .HasDefaultValueSql("nextval('\"DrillingMachines_IDDrillingMachine_seq\"'::regclass)")
                .HasColumnName("IDDrillingMachine");
            entity.Property(e => e.PositionTag).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<DrillPolygon>(entity =>
        {
            entity.HasKey(e => e.IddrillPolygon).HasName("DrillPolygon_pkey");

            entity.ToTable("DrillPolygon");

            entity.Property(e => e.IddrillPolygon).HasColumnName("IDDrillPolygon");
            entity.Property(e => e.DesignatingTag).HasMaxLength(50);
        });

        modelBuilder.Entity<PositionsDrillMachine>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PositionsDrillMachine_pkey");

            entity.ToTable("PositionsDrillMachine");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('\"DrillPositions_ID_seq\"'::regclass)")
                .HasColumnName("ID");
            entity.Property(e => e.Date).HasColumnType("timestamp(0) without time zone");
            entity.Property(e => e.IddrillMachine).HasColumnName("IDDrillMachine");
            entity.Property(e => e.PositionTag).HasMaxLength(100);

            entity.HasOne(d => d.IddrillMachineNavigation).WithMany(p => p.PositionsDrillMachines)
                .HasForeignKey(d => d.IddrillMachine)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("DrillMachine_Positions");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
