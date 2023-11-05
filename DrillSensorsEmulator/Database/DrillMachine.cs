using System;
using System.Collections.Generic;

namespace DrillSensorsEmulator.Database;

public partial class DrillMachine
{
    public int IddrillingMachine { get; set; }

    public string Title { get; set; } = null!;

    public string PositionTag { get; set; } = null!;

    public float Latitude { get; set; }

    public float Longitude { get; set; }

    public virtual ICollection<PositionsDrillMachine> PositionsDrillMachines { get; set; } = new List<PositionsDrillMachine>();
}
