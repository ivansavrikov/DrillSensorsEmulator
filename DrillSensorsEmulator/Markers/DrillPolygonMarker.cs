using GMap.NET.WindowsPresentation;
using GMap.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Effects;

namespace DrillSensorsEmulator.Markers
{
    internal class DrillPolygonMarker : GMapPolygon
    {
        private readonly GMapControl _map;
        public DrillPolygonMarker(IEnumerable<PointLatLng> points, GMapControl map) : base(points)
        {
            Shape = new Path
            {
                Stroke = Brushes.White,
                StrokeThickness = 3,
                Effect = null,
                Fill = new SolidColorBrush(Colors.Black),
                Opacity = 0.70
            };

            Shape.MouseWheel += Shape_MouseWheel;
            ZIndex = int.MinValue;
            _map = map;
        }

        private void Shape_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
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
