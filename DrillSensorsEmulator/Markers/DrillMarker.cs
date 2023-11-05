using DrillSensorsEmulator.Database;
using DrillSensorsEmulator.MarkersBodies;
using GMap.NET;
using GMap.NET.WindowsPresentation;
using System.ComponentModel.DataAnnotations;

namespace DrillSensorsEmulator.Markers
{
    class DrillMarker : GMapMarker
    {
        private bool _isCurrent = false;
        public bool IsCurrent
        {
            get
            {
                return _isCurrent;
            }

            set
            {
                _isCurrent = value;

                var body = (BodyDrillMarker1)Shape;
                body.SelectedChanged(value);
            }
        }
        public DrillMachine Drill { get; set; }
        public DrillMarker(PointLatLng pos, DrillMachine drill, GMapControl map) : base(pos)
        {
            Drill = drill;
            Shape = new BodyDrillMarker1(this, map);
        }
    }
}
