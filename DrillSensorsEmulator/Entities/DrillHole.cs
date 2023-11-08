using System;
using System.Collections.Generic;

namespace DrillSensorsEmulator.Database;

public partial class DrillHole
{
    public int IddrillHole { get; set; }

    public string DesignatingTag { get; set; } = null!;

    public int IddrillPolygon { get; set; }

    public float Latitude { get; set; }

    public float Longitude { get; set; }

    public virtual DrillPolygon IddrillPolygonNavigation { get; set; } = null!;
}
