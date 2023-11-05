using System;
using System.Collections.Generic;

namespace DrillSensorsEmulator.Database;

public partial class CoordinatesDrillPolygon
{
    public int Id { get; set; }

    public int IddrillPolygon { get; set; }

    public float Latitude { get; set; }

    public float Longitude { get; set; }

    public virtual DrillPolygon IddrillPolygonNavigation { get; set; } = null!;
}
