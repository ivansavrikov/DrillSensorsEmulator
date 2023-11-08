using GMap.NET;
using System;
using System.Collections.Generic;

namespace DrillSensorsEmulator.Database;

public partial class DrillPolygon
{
    public int IddrillPolygon { get; set; }

    public string DesignatingTag { get; set; } = null!;

    public virtual ICollection<CoordinatesDrillPolygon> CoordinatesDrillPolygons { get; set; } = new List<CoordinatesDrillPolygon>();

    public virtual ICollection<DrillHole> DrillHoles { get; set; } = new List<DrillHole>();

    public virtual ICollection<PointLatLng> DrillingPolygonCoordinatesLatLngs
    {
        get
        {
            var list = new List<PointLatLng>();
            foreach (var Coordinate in CoordinatesDrillPolygons)
            {
                list.Add(new PointLatLng(Coordinate.Latitude, Coordinate.Longitude));
            }
            return list;
        }
    }
}
