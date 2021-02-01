using EFDataAccessLibrary.DataAccess;
using EFDataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFDataAccessLibrary.Data
{
    public class Controller
    {
        public int Menu(List<string> options)
        {
            Console.WriteLine("  MENU - WeatherDataApp \n");
            int selected = 0;
            bool done = false;

                while (!done)
                {
                    for (int i = 0; i < options.Count; i++)
                    {
                        if (selected == i)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write("> ");
                        }
                        else
                        {
                            Console.Write("  ");
                        }
                        Console.WriteLine(options[i]);
                        Console.ResetColor();
                    }

                    switch (Console.ReadKey(true).Key)
                    {
                        case ConsoleKey.W:
                        case ConsoleKey.UpArrow:
                            selected = Math.Max(0, selected - 1);
                            break;
                        case ConsoleKey.S:
                        case ConsoleKey.DownArrow:
                            selected = Math.Min(options.Count - 1, selected + 1);
                            break;
                        case ConsoleKey.Enter:
                            done = true;
                            break;
                    }

                    if (!done)
                        Console.CursorTop = Console.CursorTop - options.Count;
                }

            return selected + 1;
        }
        public void DisplayData(string var, List<AggregatedWeatherData> datas)
        {
            Console.WriteLine($" Date\t| {var}");
            foreach (var d in datas)
            {
                Console.WriteLine($"{d.Date:D} | {d.AvgTemperature:N2}{d.AvgHumidity:N1} ");
            }
            Console.ReadKey();
        }
        public void DisplayMoldRisk(List<MoldData> molds)
        {
            var filtedData = molds
                .SkipWhile(m => m.MoldRisk > 100 || m.MoldRisk < 0)
                .OrderBy(o => o.MoldRisk)
                .ToList();

            Console.WriteLine(" Date           | MoldRisk (%)");

            foreach (var m in filtedData)
            {
                Console.WriteLine($"{m.Date:D} | {m.MoldRisk:N1}");
            }
            if (filtedData.Count == 0)
            {
                Console.WriteLine("Risk for mold yielded no results.");
            }
            Console.ReadKey();
        }

        internal void InformUser(string action)
        {
            switch (action)
            {
                case "exit":
                    Console.WriteLine("Press the key 'Q' to exit application!");
                    break;
                default:
                    break;
            }
        }

        public void DisplayMeteorologicalDate(DateTime season)
        {
            if (season == default)
            {
                Console.WriteLine("No meteorological season measured.");
            }
            else
            {
                Console.WriteLine($"Meteorological season started: {season:D}");
            }
        }
        public void DisplayAverageTemp(float? averageCelsius)
        {
            if (averageCelsius == null)
            {
                Console.WriteLine("Date picked yieled no results");
            }
            else
            {
                Console.WriteLine($"Average temperature: {averageCelsius:N2}");
            }
        }




    }
}
