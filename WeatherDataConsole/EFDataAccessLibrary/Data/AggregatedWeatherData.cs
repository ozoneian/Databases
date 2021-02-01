using System;
using System.Collections.Generic;
using System.Text;

namespace EFDataAccessLibrary.Data
{
    public class AggregatedWeatherData
    {
        public float? AvgTemperature { get; set; }
        public double? AvgHumidity { get; set; }
        public DateTime Date { get; set; }
        public int Sensor { get; set; }
    }
}
