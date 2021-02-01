using EFDataAccessLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace EFDataAccessLibrary.DataAccess
{
    public class WeatherContext : DbContext
    {
        public DbSet<WeatherData> WeatherDatas { get; set; }
        public DbSet<WeatherSensor> WeatherSensors { get; set; }

        public static readonly ILoggerFactory ConsoleLoggerFactory
            = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter((category, level) =>
                    category == DbLoggerCategory.Database.Command.Name
                    && level == LogLevel.Information)
                    .AddConsole();
            });
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLoggerFactory(ConsoleLoggerFactory).EnableSensitiveDataLogging() //overrides to show incoming parameter values
                .UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog = EFWeatherDb;Integrated Security=True;Connect Timeout=120;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }
    }
}
