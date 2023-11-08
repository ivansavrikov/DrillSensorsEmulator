using DrillSensorsEmulator.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DrillSensorsEmulator.Core
{
    static class ServerOperations
    {
        public static async Task<bool> SendDrillPosition(string coordinates)
        {
            //Uri serverUri = new Uri("wss://НеСуществуюшийСервер"); // Для тестирования
            //Uri serverUri = new Uri("wss://socketsbay.com/wss/v2/1/demo/"); // Для тестирования
            Uri serverUri = new Uri("ws://109.174.29.40:6686/ws/1"); // Основной

            using ClientWebSocket clientWebSocket = new();
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var timeoutTimer = new CancellationTokenSource(1*1000).Token;
                using (var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationTokenSource.Token, timeoutTimer))
                await clientWebSocket.ConnectAsync(serverUri, linkedTokenSource.Token);
                byte[] buffer = Encoding.UTF8.GetBytes(coordinates);
                await clientWebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static List<DrillMachine> GetDrillingMachines()
        {
            try
            {
                var drillMachines = new List<DrillMachine>();
                using (var db = new RitnavSystemForDrillMachinesContext())
                {
                    drillMachines = db.DrillMachines.ToList();
                }
                return drillMachines;
            }
            catch
            {
                MessageBox.Show($"Сервер недоступен, проверьте подключение к интернету");
                throw;
            }
        }

        public static List<DrillPolygon> GetDrillPolygons()
        {
            try
            {
                var polygons = new List<DrillPolygon>();
                using (var db = new RitnavSystemForDrillMachinesContext())
                {
                    polygons = db.DrillPolygons.ToList();
                    db.CoordinatesDrillPolygons.Load();
                }

                return polygons;
            }
            catch
            {
                MessageBox.Show($"Сервер недоступен, проверьте подключение к интернету");
                throw;
            }
        }

        public static List<DrillHole> GetDrillHoles()
        {
            try
            {
                var holes = new List<DrillHole>();
                using (var db = new RitnavSystemForDrillMachinesContext())
                {
                    holes = db.DrillHoles.ToList();
                }
                return holes;
            }
            catch
            {
                MessageBox.Show($"Сервер недоступен, проверьте подключение к интернету");
                throw;
            }
        }
    }
}
