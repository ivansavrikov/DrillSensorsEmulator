using GMap.NET.MapProviders;
using GMap.NET;
using System.Windows;
using System.Windows.Input;
using GMap.NET.WindowsPresentation;

namespace DrillSensorsEmulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadMap(Map); //улучшить разметку
        }

        private void LoadMap(GMapControl map)
        {
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            map.MapProvider = GoogleSatelliteMapProvider.Instance; //Сделать переключение
            map.DragButton = MouseButton.Left;
            map.ShowCenter = false;
            map.Position = new PointLatLng(54.986676, 82.949524);
        }

        #region Custom Top Panel
        //Сделать масштабирование окна
        //Режим полного окна сделать красивей
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

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                var point = e.GetPosition(this);
                this.Left = point.X - 300; // Вычислить
                this.Top = point.Y - 30; // Вычислить
            }

            this.DragMove();
        } 
        #endregion
    }
}
