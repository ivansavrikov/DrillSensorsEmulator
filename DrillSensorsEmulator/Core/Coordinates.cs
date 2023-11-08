using GMap.NET;
using System;
using System.Globalization;

namespace DrillSensorsEmulator.Core
{
    static class Coordinates
    {
        public static string ToGPGGA(PointLatLng position)
        {
            string utcTime;
            string latitude; //
            string latitudeDirection;
            string longitude; //
            string longitudeDirection;
            int fixQuality = 8; //Simulator mode
            int satellitesUsed = 0;
            double? hdop = null; //optimal 1.0
            double? altitude = null;
            string? altitudeUnits = null; // = "M"
            double? geoidSeparation = null; 
            string? geoidSeparationUnits = null; // = "M"
            double? ageOfDifferentialData = null;
            int? differentialReferenceStationID = null;

            latitudeDirection = position.Lat >= 0 ? "N" : "S";
            longitudeDirection = position.Lng >= 0 ? "E" : "W";

            position.Lat = Math.Abs(position.Lat);
            position.Lng = Math.Abs(position.Lng);

            int latDegrees = (int)position.Lat;
            double latMinutes = (position.Lat - latDegrees) * 60;

            int lonDegrees = (int)position.Lng;
            double lonMinutes = (position.Lng - lonDegrees) * 60;

            CultureInfo cultureInfo = new CultureInfo("en-US"); //Для точки в качестве десятичного разделителя
            latitude = $"{latDegrees}{latMinutes.ToString("00.0000", cultureInfo)}";
            longitude = $"{lonDegrees}{lonMinutes.ToString("00.0000", cultureInfo)}";

            DateTime timeNow = DateTime.Now;
            utcTime = timeNow.ToString("HHmmss.ff");

            string gpggaString = 
                $"$GPGGA," +
                $"{utcTime}," +
                $"{latitude}," +
                $"{latitudeDirection}," +
                $"{longitude}," +
                $"{longitudeDirection}," +
                $"{fixQuality}," +
                $"{satellitesUsed:00}," +
                $"{hdop?.ToString("F1", cultureInfo)}," + //hdop = null
                $"{altitude?.ToString("F1", cultureInfo)}," + //altitude = null
                $"{altitudeUnits}," + //altitudeUnits = null
                $"{geoidSeparation?.ToString("F1", cultureInfo)}," + //geoidSeparation = null
                $"{geoidSeparationUnits}," + //geoidSeparationUnits = null
                $"{ageOfDifferentialData:00}," + //ageOfDifferentialData = null
                $"{differentialReferenceStationID:0000}*"; //differentialReferenceStationID = null

            return CalculateNMEAChecksum(gpggaString);
        }

        private static string CalculateNMEAChecksum(string sentence)
        {
            int startIndex = sentence.IndexOf('$');
            int endIndex = sentence.IndexOf('*');

            string data = sentence.Substring(startIndex + 1, endIndex - startIndex - 1);
            byte checksum = 0;

            for (int i = 0; i < data.Length; i++)
            {
                checksum ^= (byte)data[i];
            }

            string checksumHex = checksum.ToString("X2");
            return $"{sentence}{checksumHex}";
        }

        public static string ToSimplePositionMessage(PointLatLng position, int id)
        {
            DateTime timeNow = DateTime.Now;
            
            return
                $"{id}," +
                $"{timeNow:HHmmss.ff}," +
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