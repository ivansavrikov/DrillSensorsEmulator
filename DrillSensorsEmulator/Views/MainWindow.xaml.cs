using GMap.NET.MapProviders;
using GMap.NET;
using System.Windows;
using System.Windows.Input;
using GMap.NET.WindowsPresentation;
using DrillSensorsEmulator.Markers;
using System.Windows.Media;
using System.Collections.Generic;
using DrillSensorsEmulator.MarkersBodies;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using DrillSensorsEmulator.Database;
using System.Linq;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Printing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Globalization;
using DrillSensorsEmulator.Core;

namespace DrillSensorsEmulator.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadMap(Map);
            LoadDrillMarkers(Map);
        }

        private void LoadMap(GMapControl map)
        {
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            map.MapProvider = GoogleSatelliteMapProvider.Instance; //Сделать переключение
            map.DragButton = MouseButton.Left;
            map.ShowCenter = false;
            map.Position = new PointLatLng(54.986676, 82.949524); //
        }

        private void LoadDrillMarkers(GMapControl map)
        {
            using var db = new RitnavSystemForDrillMachinesContext();
            var drills = db.DrillMachines.ToList();

            foreach (var drill in drills)
            {
                var point = new PointLatLng(drill.Latitude, drill.Longitude);

                DrillMarker marker = new DrillMarker(point, drill, map);
                BodyDrillMarker1 markerBody = (BodyDrillMarker1)marker.Shape;

                markerBody.MarkerMouseLeftButtonDown += DrillMarker_MarkerMouseLeftButtonDown;
                markerBody.MarkerMouseRightDown += MarkerBody_MarkerMouseRightDown;
                markerBody.MouseMove += MarkerBody_MouseMove;

                map.Markers.Add(marker);
            }
        }

        private void MarkerBody_MarkerMouseRightDown(object sender, EventArgs e)
        {
            SetCurrentMarker((BodyDrillMarker1)sender);
        }

        private void ShowDrillPosition(DrillMarker marker)
        {
            if (marker.IsCurrent)
            {
                tbLat.Text = Math.Round(marker.Position.Lat, 6).ToString().Replace(",", ".");
                tbLon.Text = Math.Round(marker.Position.Lng, 6).ToString().Replace(",", ".");
            }
        }
        private void MarkerBody_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is BodyDrillMarker1 body)
            {
                ShowDrillPosition(body.Marker);
            }
        }

        private void SetCurrentMarker(BodyDrillMarker1 body)
        {
            foreach (DrillMarker m in Map.Markers.Cast<DrillMarker>())
            {
                m.IsCurrent = false;
            }

            lDrill.Content = body.Marker.Drill.Title;
            body.Marker.IsCurrent = true;
            ShowDrillPosition(body.Marker);
        }
        private void DrillMarker_MarkerMouseLeftButtonDown(object sender, System.EventArgs e)
        {
            SetCurrentMarker((BodyDrillMarker1)sender);
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

        private void BtnShowPositionPanel_Click(object sender, RoutedEventArgs e)
        {
            PositionPanel.Visibility = PositionPanel.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
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

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
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

                    foreach(DrillMarker m in Map.Markers.Cast<DrillMarker>())
                    {
                        if (m.IsCurrent)
                        {
                            m.Position = new PointLatLng(lat, lon);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"{ex.Message}");
                }

                e.Handled = true;
            }
        }

        private void BtnSendPosition_Click(object sender, RoutedEventArgs e)
        {
            foreach(DrillMarker m in Map.Markers.Cast<DrillMarker>())
            {
                if (m.IsCurrent)
                {
                    _ = ServerOperations.SendDrillPosition(Coordinates.ToSimplePositionMessage(m.Position.Lat, m.Position.Lng, m.Drill.IddrillingMachine));
                }
            }
        }

        //Режим автоматического перемещения бура
        private void BtnAutoSendMode_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Еще не реализовано");
        }
    }
}