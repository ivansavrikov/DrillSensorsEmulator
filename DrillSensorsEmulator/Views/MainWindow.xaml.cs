using GMap.NET.MapProviders;
using GMap.NET;
using System.Windows;
using System.Windows.Input;
using DrillSensorsEmulator.Markers;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Linq;
using System;
using System.Globalization;
using DrillSensorsEmulator.Core;
using System.Windows.Threading;
using DrillSensorsEmulator.Database;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DrillSensorsEmulator.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer _timer;
        private DrillMarker _currentDrillMarker;
        internal DrillMarker CurrentDrillMarker //
        {
            get
            {
                return _currentDrillMarker;
            }
            set
            {
                _currentDrillMarker = value;
                btnDrills.Content = _currentDrillMarker.Drill.Title;

                foreach(var marker in Map.Markers)
                {
                    if(marker is DrillMarker drillMarker)
                    {
                        drillMarker.IsCurrent = false;
                    }
                }

                _currentDrillMarker.IsCurrent = true;
                ShowDrillPosition(_currentDrillMarker);
            }
        }

        private int _changeMapProviderClicks = 0;

        public MainWindow()
        {
            InitializeComponent();
            LoadMap();
            LoadDrillMarkers();
            LoadDrillPolygonsMarkers();
            LoadHoleMarkers();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(5);
            _timer.Tick += AutoMove_TimerTick;

            CurrentDrillMarker = (DrillMarker)Map.Markers.First(); //

            lvDrills.ItemsSource = ServerOperations.GetDrillingMachines();
        }
        private void LoadMap()
        {
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            Map.MapProvider = GoogleSatelliteMapProvider.Instance; //Сделать переключение
            Map.DragButton = MouseButton.Left;
            Map.ShowCenter = false;
            Map.Position = new PointLatLng(54.986676, 82.949524); //
        }

        private void LoadDrillMarkers()
        {
            var drills = ServerOperations.GetDrillingMachines();

            foreach (var drill in drills)
            {
                PointLatLng point = new(drill.Latitude, drill.Longitude);
                DrillMarker marker = new(point, drill, Map);

                marker.MouseLeftDown += Marker_MouseLeftDown;
                marker.MouseRightDown += Marker_MouseRightDown;
                marker.MouseRightUp += Marker_MouseRightUp;
                marker.MouseMove += Marker_MouseMove;

                Map.Markers.Add(marker);
            }
        }

        private void LoadDrillPolygonsMarkers()
        {
            try
            {
                using var db = new RitnavSystemForDrillMachinesContext();
                var listPotygons = db.DrillPolygons.ToList();
                db.CoordinatesDrillPolygons.Load();
                foreach (var polygon in listPotygons)
                {
                    var marker = new DrillPolygonMarker(polygon.DrillingPolygonCoordinatesLatLngs, Map);
                    Map.Markers.Add(marker);
                }
            }
            catch
            {
                MessageBox.Show("Ошибка загрузки полигонов", "ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadHoleMarkers()
        {
            using var db = new RitnavSystemForDrillMachinesContext();
            var holes = db.DrillHoles.ToList();

            foreach(var hole in holes)
            {
                var point = new PointLatLng(hole.Latitude, hole.Longitude);
                var marker = new HoleMarker(point, Map);
                Map.Markers.Add(marker);
            }
        }

        private void Marker_MouseMove(object sender, MouseEventArgs e)
        {
            ShowDrillPosition(CurrentDrillMarker);
        }

        private async void Marker_MouseRightUp(object sender, MouseButtonEventArgs e)
        {
            if (tbtnAutoSendPosition.IsChecked == true)
            {
                await Task.Run(() => StartSending());
            }
        }

        private void Marker_MouseRightDown(object sender, MouseButtonEventArgs e)
        {
            CurrentDrillMarker = (DrillMarker)sender;
        }

        private void Marker_MouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            CurrentDrillMarker = (DrillMarker)sender;
        }

        private void SyncWithDatabase()
        {
            Map.Markers.Clear();
            LoadDrillMarkers();
            LoadDrillPolygonsMarkers();
            LoadHoleMarkers();

            foreach (var marker in Map.Markers)
            {
                if (marker is DrillMarker drillMarker)
                {
                    if(drillMarker.Drill.IddrillingMachine == CurrentDrillMarker.Drill.IddrillingMachine)
                    {
                        CurrentDrillMarker = drillMarker;
                    }
                }
            }

            Map.Position = CurrentDrillMarker.Position;
        }
        
        //Отображение широты и долготы бура в соответсвующих текстбоксах 
        private void ShowDrillPosition(DrillMarker marker)
        {
            tbLat.Text = Math.Round(marker.Position.Lat, 6).ToString().Replace(",", ".");
            tbLon.Text = Math.Round(marker.Position.Lng, 6).ToString().Replace(",", ".");
        }

        private void BtnShowEmulatedParametersPanel_Click(object sender, RoutedEventArgs e)
        {
            ChangeVisibilityEmulatedParametersPanel();
        }

        private void ChangeVisibilityEmulatedParametersPanel()
        {
            EmulatedParametersPanel.Visibility = EmulatedParametersPanel.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Lat_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            //текст до и после добавления символа
            string textBefore = textBox.Text.Substring(0, textBox.SelectionStart);
            string textAfter = textBox.Text.Substring(textBox.SelectionStart + textBox.SelectionLength);

            // Предотвращаем ввод символов, которые нарушают формат
            if (!Regex.IsMatch(textBefore + e.Text + textAfter, @"^-?\d{0,2}(\.\d{0,6})?$"))
            {
                e.Handled = true; // Предотвращаем ввод символа
            }
        }

        private void Lon_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            //текст до и после добавления символа
            string textBefore = textBox.Text.Substring(0, textBox.SelectionStart);
            string textAfter = textBox.Text.Substring(textBox.SelectionStart + textBox.SelectionLength);

            // Предотвращаем ввод символов, которые нарушают формат
            if (!Regex.IsMatch(textBefore + e.Text + textAfter, @"^-?\d{0,3}(\.\d{0,6})?$"))
            {
                e.Handled = true; // Предотвращаем ввод символа
            }
        }

        private async void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Keyboard.ClearFocus();

                CultureInfo culture = CultureInfo.InvariantCulture;
                try
                {
                    var latStr = tbLat.Text.ToString();
                    var lonStr = tbLon.Text.ToString();

                    var lat = double.Parse(latStr, culture);
                    var lon = double.Parse(lonStr, culture);

                    CurrentDrillMarker.Position = new PointLatLng(lat, lon);

                    if (tbtnAutoSendPosition.IsChecked == true)
                    {
                        await Task.Run(() => StartSending());
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"{ex.Message}");
                }

                e.Handled = true;
            }
        }

        /// <summary>
        /// Переключение между видами карты: Гибрид, Схема, Спутник.
        /// </summary>
        private void ChangeMapProvider_Click(object sender, RoutedEventArgs e)
        {
            switch (_changeMapProviderClicks % 3)
            {
                case 0: Map.MapProvider = GoogleHybridMapProvider.Instance; break;
                case 1: Map.MapProvider = GoogleMapProvider.Instance;  break;
                case 2: Map.MapProvider = GoogleSatelliteMapProvider.Instance; break;
                default: break;
            }
            _changeMapProviderClicks++;
        }

        private async void AutoMove_TimerTick(object? sender, EventArgs e)
        {
            CurrentDrillMarker.Position = Coordinates.GenerateRandomPoint(CurrentDrillMarker.Position);
            Map.Position = CurrentDrillMarker.Position;

            if (tbtnAutoSendPosition.IsChecked == true)
            {
                await Task.Run(() => StartSending());
            }

            ShowDrillPosition(CurrentDrillMarker);
        }

        private void AutoSendMode_Checked(object sender, RoutedEventArgs e)
        {
            _timer.Start();
        }

        private void AutoSendMode_Unchecked(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
        }

        async void BtnSendPosition_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => StartSending());
        }

        private async Task StartSending()
        {
            var currentDrillPosition = new PointLatLng(CurrentDrillMarker.Drill.Latitude, CurrentDrillMarker.Drill.Longitude);

            if (CurrentDrillMarker.Position.Lat == currentDrillPosition.Lat && CurrentDrillMarker.Position.Lng == currentDrillPosition.Lng)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    lSendingStatus.Content = "the drill didn't move!";
                });

                await Task.Delay(500);

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    lSendingStatus.Content = "";
                });
                return;
            }

            bool? isEnabledAutoSendingMode = false;

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                isEnabledAutoSendingMode = tbtnAutoSendPosition.IsChecked;

                lSendingStatus.Foreground = new SolidColorBrush(Colors.LightGray);
                lSendingStatus.Content = "waiting...";
                tbtnAutoSendPosition.IsChecked = false;
                tbtnAutoSendPosition.IsEnabled = false;
                btnSendPosition.IsEnabled = false;
            });

            bool success = await ServerOperations.SendDrillPosition($"{CurrentDrillMarker.Drill.IddrillingMachine}{Coordinates.ToGPGGA(CurrentDrillMarker.Position)}");

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                if (success)
                {
                    lSendingStatus.Foreground = new SolidColorBrush(Colors.MediumSeaGreen);
                    lSendingStatus.Content = "successfully.";
                }
                else
                {
                    lSendingStatus.Foreground = new SolidColorBrush(Colors.IndianRed);
                    lSendingStatus.Content = "error.";
                }
            });

            await Task.Delay(500);

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                lSendingStatus.Foreground = new SolidColorBrush(Colors.LightGray);
                lSendingStatus.Content = "";
                tbtnAutoSendPosition.IsEnabled = true;
                tbtnAutoSendPosition.IsChecked = isEnabledAutoSendingMode;
                btnSendPosition.IsEnabled = true;
            });

            await Task.Delay(500); //время серверу на обработку

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                SyncWithDatabase();
            });
        }

        private void BtnSyncDatabase_Click(object sender, RoutedEventArgs e)
        {
            SyncWithDatabase();
        }

        private void btnDrills_Click(object sender, RoutedEventArgs e)
        {
            lvDrills.Visibility = lvDrills.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        #region Custom Top Panel
        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnMaximize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = (this.WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        #endregion
    }
}