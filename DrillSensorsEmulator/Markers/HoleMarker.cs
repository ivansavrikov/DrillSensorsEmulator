using GMap.NET;
using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillSensorsEmulator.Markers
{
    internal class HoleMarker : GMapMarker
    {
        private readonly GMapControl _map;
        public HoleMarker(PointLatLng pos, GMapControl map) : base(pos)
        {
            var body = new HoleMarkerBody();
            body.MouseDoubleClick += Body_MouseDoubleClick;
            body.MouseWheel += Body_MouseWheel;
            Shape = body;
            _map = map;
            ZIndex = int.MinValue + 1000;
        }

        private void Body_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _map.Position = Position;
            _map.Zoom = 16;
        }

        private void Body_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                _map.Zoom += 1;
            }
            else
            {
                _map.Zoom -= 1;
            }
        }
    }
}
