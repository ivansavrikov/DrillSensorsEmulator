using DrillSensorsEmulator.Database;
using GMap.NET;
using GMap.NET.WindowsPresentation;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace DrillSensorsEmulator.Markers
{
    class DrillMarker : GMapMarker
    {
        public DrillMachine Drill { get; set; }
        private readonly GMapControl _map;
        public DrillMarkerBody _body;
        private bool _isCurrent;

        public bool IsCurrent
        {
            get
            {
                return _isCurrent;
            }
            set
            {
                _isCurrent = value;

                if (_isCurrent)
                {
                    _body.MarkerView.Fill = new SolidColorBrush(Colors.IndianRed);
                }
                else
                {
                    _body.MarkerView.Fill = new SolidColorBrush(Colors.LightGray);
                }
            }
        }

        public event MouseButtonEventHandler? MouseLeftDown;
        public event MouseButtonEventHandler? MouseRightDown;
        public event MouseButtonEventHandler? MouseRightUp;
        public event MouseEventHandler? MouseMove;
        public DrillMarker(PointLatLng pos, DrillMachine drill, GMapControl map) : base(pos)
        {
            Drill = drill;
            _map = map;

            _body = new DrillMarkerBody();
            _body.Hint.Content = Drill.Title;
            _body.Loaded += MarkerLoaded;
            _body.MouseEnter += MarkerMouseEnter;
            _body.MouseLeave += MarkerMouseLeave;
            _body.MouseLeftButtonDown += MarkerMouseLeftButtonDown;
            _body.MouseLeftButtonUp += MarkerMouseLeftButtonUp;
            _body.MouseRightButtonDown += MarkerMouseRightDown;
            _body.MouseRightButtonUp += MarkerMouseRightUp;
            _body.MouseMove += MarkerMouseMove;
            _body.MouseWheel += MarkerMouseWheel;
            _body.MouseDoubleClick += MarkerMouseDoubleClick;

            Shape = _body;
        }

        private void MarkerMouseDoubleClick(object? sender, MouseButtonEventArgs e)
        {
            _map.Position = Position;
            _map.Zoom = 16;
        }

        private void MarkerMouseWheel(object? sender, MouseWheelEventArgs e)
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

        private void MarkerMouseMove(object? sender, MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed && _body.IsMouseCaptured)
            {
                var cursor = e.GetPosition(_map);
                var newPosition = _map.FromLocalToLatLng((int)cursor.X, (int)cursor.Y);
                Position = newPosition;
            }
            MouseMove?.Invoke(this, e);
        }

        private void MarkerMouseRightUp(object? sender, MouseButtonEventArgs e)
        {
            if (_body.IsMouseCaptured)
            {
                Mouse.Capture(null);
            }
            MouseRightUp?.Invoke(this, e);
        }

        private void MarkerMouseRightDown(object? sender, MouseButtonEventArgs e)
        {
            if (!_body.IsMouseCaptured)
            {
                Mouse.Capture(_body);
            }
            _body.PopupHint.IsOpen = false;
            MouseRightDown?.Invoke(this, e);
        }

        private void MarkerMouseLeftButtonUp(object? sender, MouseButtonEventArgs e)
        {
            //
        }

        private void MarkerMouseLeftButtonDown(object? sender, MouseButtonEventArgs e)
        {
            _body.PopupHint.IsOpen = false;
            MouseLeftDown?.Invoke(this, e);
        }

        private void MarkerMouseLeave(object? sender, MouseEventArgs e)
        {
            ZIndex -= 10000;
            _body.MarkerView.Stroke = new SolidColorBrush(Colors.Black);
            _body.PopupHint.IsOpen = false;
        }

        private void MarkerMouseEnter(object? sender, MouseEventArgs e)
        {
            ZIndex += 10000;           
            _body.MarkerView.Stroke = new SolidColorBrush(Colors.IndianRed);
            _body.PopupHint.IsOpen = true;
        }

        private void MarkerLoaded(object? sender, RoutedEventArgs e)
        {   
            Offset = new Point(-_body.Height / 2, -_body.Width / 2);
        }

    }
}
