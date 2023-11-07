using GMap.NET;
using System;
using System.Globalization;

namespace DrillSensorsEmulator.Core
{
    static class Coordinates
    {
        public static string ToGPGGA(double lat, double lon, int id) //не полный GPGGA, для задания достаточно
        {
            string latDir = lat >= 0 ? "N" : "S";
            string lonDir = lon >= 0 ? "E" : "W";

            lat = Math.Abs(lat);
            lon = Math.Abs(lon);

            int latDegrees = (int)lat;
            double latMinutes = (lat - latDegrees) * 60;

            int lonDegrees = (int)lon;
            double lonMinutes = (lon - lonDegrees) * 60;

            DateTime timeNow = DateTime.Now;

            CultureInfo cultureInfo = new CultureInfo("en-US"); //Для точки в качестве десятичного разделителя
            
            return
                $"{id},$GPGGA,{timeNow.ToString("HHmmss.ff")}," +
                $"{latDegrees}{latMinutes.ToString("00.0000", cultureInfo)},{latDir}," +
                $"{lonDegrees}{lonMinutes.ToString("00.0000", cultureInfo)},{lonDir}";

        }

        public static string ToSimplePositionMessage(PointLatLng position, int id)
        {
            DateTime timeNow = DateTime.Now;
            
            return
                $"{id}," +
                $"{timeNow}," +
                $"{Math.Round(position.Lat, 10).ToString().Replace(',', '.')}," +
                $"{Math.Round(position.Lng, 10).ToString().Replace(',', '.')}";
        }

        public static PointLatLng GenerateRandomPoint(PointLatLng point)
        {
            var currentLat = point.Lat;
            var currentLon = point.Lng;

            // Радиус смещения (в градусах)
            double radiusInDegrees = 0.0100;

            Random random = new();

            // Генерирация случайного смещения в пределах радиуса
            double latitudeOffset = (random.NextDouble() - 0.5) * radiusInDegrees;
            double longitudeOffset = (random.NextDouble() - 0.5) * radiusInDegrees;

            // Новые координаты
            double newLat = currentLat + latitudeOffset;
            double newLon = currentLon + longitudeOffset;

            return new PointLatLng(Math.Round(newLat, 6), Math.Round(newLon, 6));
        }
    }
}