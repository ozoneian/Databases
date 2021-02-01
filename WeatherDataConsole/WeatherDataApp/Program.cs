using System;
using EFDataAccessLibrary.Data;
using EFDataAccessLibrary.DataAccess;

namespace WeatherDataApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var app = new App();
            app.Start();
        }
    }
}
