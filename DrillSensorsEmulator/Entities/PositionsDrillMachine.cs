using System;

namespace DrillSensorsEmulator.Database;

public partial class PositionsDrillMachine
{
    public int Id { get; set; }

    public int IddrillMachine { get; set; }

    public string PositionTag { get; set; } = null!;

    public float Latitude { get; set; }

    public float Longitude { get; set; }

    public DateTime Date { get; set; }

    public virtual DrillMachine IddrillMachineNavigation { get; set; } = null!;
}
