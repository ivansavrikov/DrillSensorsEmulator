using DrillSensorsEmulator.Database;
using DrillSensorsEmulator.Markers;
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
        public static async Task SendDrillPosition(string coordinates)
        {
            Uri serverUri = new Uri("wss://socketsbay.com/wss/v2/1/demo/"); // Заменить
            //Uri serverUri = new Uri("ws://109.174.29.40:6686/ws/1");

            using (ClientWebSocket clientWebSocket = new())
            {
                try
                {
                    await clientWebSocket.ConnectAsync(serverUri, CancellationToken.None);
                    byte[] buffer = Encoding.UTF8.GetBytes(coordinates);
                    await clientWebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при отправке данных: {ex.Message}");
                }
            }
        }
    }
}
