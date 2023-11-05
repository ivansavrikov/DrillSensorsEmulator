using DrillSensorsEmulator.Markers;
using GMap.NET.WindowsPresentation;
using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace DrillSensorsEmulator.MarkersBodies
{
    /// <summary>
    /// Логика взаимодействия для BodyDrillMarker1.xaml
    /// </summary>
    public partial class BodyDrillMarker1 : UserControl
    {
        internal DrillMarker Marker { get; set; }
        private readonly GMapControl _map;
        private readonly Popup _popup;
        private readonly Label _label;

        public event EventHandler? MarkerMouseEnter;
        public event EventHandler? MarkerMouseLeave;
        public event EventHandler? MarkerMouseLeftButtonDown;
        public event EventHandler? MarkerMouseLeftButtonUp;
        public event EventHandler? MarkerMouseRightDown;
        public event EventHandler? MarkerMouseRightUp;
        public event EventHandler? MarkerMouseMove;
        public event EventHandler? MarkerMouseWheel;

        internal BodyDrillMarker1(DrillMarker marker, GMapControl map)
        {
            InitializeComponent();
            Marker = marker;
            _map = map;

            Loaded += DrillMarker_Loaded;
            MouseEnter += DrillMarker_MouseEnter;
            MouseLeave += DrillMarker_MouseLeave;
            MouseLeftButtonDown += DrillMarker_MouseLeftButtonDown;
            MouseLeftButtonUp += DrillMarker_MouseLeftButtonUp;
            MouseRightButtonDown += DrillMarkerUI_MouseRightButtonDown;
            MouseRightButtonUp += DrillMarkerUI_MouseRightButtonUp;
            MouseMove += DrillMarker_MouseMove;
            MouseWheel += DrillMarkerUI_MouseWheel;
            MouseDoubleClick += BodyDrillMarker_MouseDoubleClick;

            _popup = new Popup();
            _label = new Label();

            _popup.Placement = PlacementMode.Mouse;
            {
                _label.Background = Brushes.DarkGray;
                _label.Foreground = Brushes.White;
                _label.BorderBrush = Brushes.Black;
                _label.BorderThickness = new Thickness(2);
                _label.Padding = new Thickness(5);
                _label.FontSize = 14;
                _label.FontFamily = new FontFamily("pack://application:,,,/DrillSensorsEmulator;component/Assets/Fonts/#Montserrat Regular"); //
                _label.FontStyle = FontStyles.Italic;
                _label.FontWeight = FontWeights.Bold;
                _label.Content = marker.Drill.Title;
            }
            _popup.Child = _label;
        }

        public void SelectedChanged(bool isCurrent)
        {
            if (isCurrent)
            {
                MarkerView.Fill = new SolidColorBrush(Colors.IndianRed);
            }
            else
            {
                MarkerView.Fill = new SolidColorBrush(Colors.LightGray);
            }
        }

        private void BodyDrillMarker_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _map.Position = Marker.Position;
            _map.Zoom = 14;
        }

        private void DrillMarker_Loaded(object sender, RoutedEventArgs e)
        {
            Marker.Offset = new Point(-this.Height / 2, -this.Width / 2);
        }

        private void DrillMarkerUI_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            MarkerMouseWheel?.Invoke(this, EventArgs.Empty);
        }

        private void DrillMarkerUI_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
            {
                Mouse.Capture(null);
            }

            MarkerMouseRightUp?.Invoke(this, EventArgs.Empty);
        }

        private void DrillMarkerUI_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsMouseCaptured)
            {
                Mouse.Capture(this);
            }
            _popup.IsOpen = false;

            MarkerMouseRightDown?.Invoke(this, EventArgs.Empty);
        }

        private void DrillMarker_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed && IsMouseCaptured)
            {
                var cursor = e.GetPosition(_map);
                var newPosition = _map.FromLocalToLatLng((int)cursor.X, (int)cursor.Y);
                Marker.Position = newPosition;
            }

            MarkerMouseMove?.Invoke(this, EventArgs.Empty);
        }

        private void DrillMarker_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MarkerMouseLeftButtonUp?.Invoke(this, EventArgs.Empty);
        }

        private void DrillMarker_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MarkerMouseLeftButtonDown?.Invoke(this, EventArgs.Empty);
        }

        private void DrillMarker_MouseLeave(object sender, MouseEventArgs e)
        {
            Marker.ZIndex -= 10000;
            _popup.IsOpen = false;

            MarkerMouseLeave?.Invoke(this, EventArgs.Empty);
        }

        private void DrillMarker_MouseEnter(object sender, MouseEventArgs e)
        {
            Marker.ZIndex += 10000;
            _popup.IsOpen = true;

            MarkerMouseEnter?.Invoke(this, EventArgs.Empty);
        }
    }
}
