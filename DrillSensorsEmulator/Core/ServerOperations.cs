using DrillSensorsEmulator.Database;
using System;
using System.Collections.Generic;
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
            Uri serverUri = new Uri("wss://socketsbay.com/wss/v2/1/demo/"); // Для тестирования
            //Uri serverUri = new Uri("ws://109.174.29.40:6686/ws/1"); // Основной

            using ClientWebSocket clientWebSocket = new();
            try
            {
                await clientWebSocket.ConnectAsync(serverUri, CancellationToken.None);
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
                using (var context = new RitnavSystemForDrillMachinesContext())
                {
                    drillMachines = context.DrillMachines.ToList();
                }
                return drillMachines;
            }
            catch
            {
                MessageBox.Show($"Сервер недоступен, проверьте подключение к интернету");
                throw;
            }
        }
    }
}
