using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EFDataAccessLibrary.Models
{
    [Table("WeatherDatas")]
    public class WeatherData
    {
        public int Id { get; set; } //'uneccessary' -> allows for linking in the future
        public byte? Humidity { get; set; } // 1 byte tinyint 0-255 (humidity: 0-100%)
        public float? Temperature { get; set; } // 4 bytes - 6-9 digits precision (temperature: usually less accurate to measure so less precision is fine)
        //both humidity and temp --> nullable to allow for a sensor to be added that only measures temp/humidity. Allows for the data to still be considered valid.
        [Required] 
        public DateTime Collected_On { get; set; } //same logic as with location --> required is added because the data would be "unusable" without it.

        [ForeignKey("WeatherSensor")] 
        public int WeatherSensorId { get; set; }
        public virtual WeatherSensor WeatherSensor { get; set; }
    }
}
