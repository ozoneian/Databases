using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EFDataAccessLibrary.Models
{
    [Index(nameof(Location), IsUnique = true)]
    [Table("WeatherSensors")]

    public class WeatherSensor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(64)] //To avoid nvarchar(max)

        public string Location { get; set; } //Without the specified location the data would be "unusable" - required

        [Required]
        public virtual ICollection<WeatherData> WeatherDatas { get; set; } //one to many relationship between sensor and data.

        //Extracting the sensors from the data allows for another sensor to be added with ease. Prevents the same data from being repeated "(ute/inne)".
    }
}
