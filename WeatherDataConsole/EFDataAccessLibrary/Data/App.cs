using EFDataAccessLibrary.DataAccess;
using EFDataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFDataAccessLibrary.Data
{
    public class App
    {
        private readonly Controller _nav;
        private readonly WeatherContext _context;
        private List<string> DisplayOptions { get; set; } = new List<string> 
        { 
            "Average temperature for a date (search by date)",
            "Average temperature for all dates (hot -> cold)",
            "Average humidity for all dates (arid -> humid",
            "Risk of mold for all dates (low -> high)", 
            "Meteorological autumn (start-date)", 
            "Meteorological winter (start-date)" 
        };

        public App()
        {
            _context = new WeatherContext();
            _nav = new Controller();

           LoadWeatherData();
        }

        public void Start()
        {
            var sensors = _context
               .WeatherSensors
               .Select(s => s.Location)
               .ToList();

            ConsoleKey keyChoice = default;

            while (keyChoice != ConsoleKey.Q)
            {
                switch (_nav.Menu(DisplayOptions))
                {
                    case 1:
                        Console.WriteLine($"Enter a date ('YYYY-MM-DD'): ");
                        string input = Console.ReadLine();
                        _nav.DisplayAverageTemp(AverageTempDay(input, _nav.Menu(sensors)));
                        break;

                    case 2:
                        _nav.DisplayData("Average Temperature (Celsius)", AverageTemperature(LocationDataByDate(_nav.Menu(sensors))));
                        break;

                    case 3:
                        _nav.DisplayData("Average Humidity", AverageHumidity(LocationDataByDate(_nav.Menu(sensors))));
                        break;

                    case 4:
                        _nav.DisplayMoldRisk(MoldRisk(LocationDataByDate(_nav.Menu(sensors))));
                        break;

                    case 5:
                        _nav.DisplayMeteorologicalDate(FindAuthumn(LocationDataByDate(2)));
                        break;

                    case 6:
                        _nav.DisplayMeteorologicalDate(FindWinter(LocationDataByDate(2)));
                        break;
                    default:
                        break;
                }
                _nav.InformUser("exit");
                    keyChoice = Console.ReadKey().Key;
            }
        }
        public void LoadWeatherData() //populate db
        {
            if (_context.WeatherSensors.Any())
            {
                return;
            }

            string file = System.IO.File.ReadAllText(@"C:\IT-Högskolan\Databaser\Entity Framework\Inlämningsuppgift\WeatherDataConsole\EFDataAccessLibrary\Data\tempdata.csv");
            var datas = new List<WeatherData>();
            var sensors = new List<WeatherSensor>();

            var records = file.Split('\n');
            foreach (var record in records)
            {
                var props = record.Split(',');
                if (!sensors.Any(s => s.Location == props[1]))
                {
                    //create sensor...
                    var sensor = new WeatherSensor { Location = props[1] };
                    _context.Add(sensor);
                    _context.SaveChanges();
                    sensors.Add(sensor);
                }

                var ws = sensors.Where(s => s.Location == props[1]).FirstOrDefault();

                var data = new WeatherData
                {
                    Collected_On = DateTime.Parse(props[0]),
                    Temperature = float.Parse(props[2], System.Globalization.CultureInfo.InvariantCulture),
                    Humidity = byte.Parse(props[3]),
                    WeatherSensor = ws,
                    WeatherSensorId = ws.Id
                };
                datas.Add(data);
            }
            _context.BulkInsert(datas);
        }

        private List<MoldData> MoldRisk(List<AggregatedWeatherData> dataByDate)
        {
            // mögelrisk = ((fuktighet - 78) * (temperatur / 15)) / 0,22
            //grov utifrån avg per day:
            var moldrisks = new List<MoldData>();

            foreach (var date in dataByDate)
            {
                var moldrisk = ((date.AvgHumidity - 78) * (date.AvgTemperature / 15)) / 0.22;
                var moldDataEntity = new MoldData
                {
                    MoldRisk = moldrisk,
                    Date = date.Date
                };
                moldrisks.Add(moldDataEntity);
            }
            var moldSorted = moldrisks
                .OrderBy(o => o.MoldRisk)
                .ToList();
            return moldSorted;
        }
        private List<AggregatedWeatherData> LocationDataByDate(int sensor)
        {
                var getAllData = _context.WeatherDatas
                    .Where(s => s.WeatherSensorId == sensor)
                    .ToList();

                var groupByDate = getAllData
                    .GroupBy(g => g.Collected_On.Date,
                    (key, g) => new AggregatedWeatherData
                    {
                        Date = key,
                        AvgTemperature = g.Average(a => a.Temperature),
                        AvgHumidity = g.Average(a => a.Humidity),
                        Sensor = sensor
                    })
                    .ToList(); // <--- startar queryn till db och laddar enbart ner den data som vi specificerat i vår where-clause.
            
            return groupByDate;
        }
        private DateTime FindAuthumn(List<AggregatedWeatherData> dataByDate)
        {
            //Höst
            //Hösten anländer om det råder hösttemperatur 5 dygn i följd.
            //Hösttemperatur är det då dygnsmedeltemperaturen är under 10,0°C, men ännu inte 5 dygn i följd.
            //Den 1 augusti har satts som det tidigaste tillåtna datumet för höstens ankomst.

            DateTime pastDate = default;
            DateTime autumn = default;
            int belowTen = 0;
            int autumnRule = 4;
            foreach (var date in dataByDate.OrderBy(o => o.Date))
            {
                if (date.AvgTemperature <= 10 && (date.Date == pastDate.Date.AddDays(1) || pastDate.Date == default))
                {
                    belowTen++;
                    if (belowTen > autumnRule)
                    {
                        autumn = date.Date.AddDays(-autumnRule);
                        break;
                    }
                }
                else
                {
                    belowTen = 0;
                }
                pastDate = date.Date;

            }
            return autumn;
        }
        private DateTime FindWinter(List<AggregatedWeatherData> dataByDate)
        {
            //Vintern anländer om det råder vintertemperatur 5 dygn i följd.
            //Vintertemperatur är det då dygnsmedeltemperaturen är 0,0°C eller lägre, men ännu inte 5 dygn i följd.
            DateTime winter = default;
            DateTime pastDate = default;
            int belowZero = 0;
            int winterRule = 4;
            foreach (var date in dataByDate)
            {
                if (date.AvgTemperature <= 0)
                {
                    belowZero++;
                    if (belowZero > winterRule && (date.Date == pastDate.Date.AddDays(1) || pastDate.Date == default))
                    {
                        winter = date.Date.AddDays(-winterRule);
                        break;
                    }
                }
                else
                {
                    belowZero = 0;
                }
                pastDate = date.Date;
            }
            return winter;
        }

        private float? AverageTempDay(string date, int sensor)
        {
            float? avgTemp;
            var success = DateTime.TryParse(date, out DateTime result);
            if (!success)
            {
                avgTemp = null;
            }
            else
            {
                avgTemp = _context.WeatherDatas
                     .Where(s => s.WeatherSensorId == sensor && s.Collected_On.Date == result)
                     .Average(a => a.Temperature);
            }
            return avgTemp;
        }
        private List<AggregatedWeatherData> AverageHumidity(List<AggregatedWeatherData> dataByDate)
        {
            var sortedData = dataByDate
                .OrderBy(o => o.AvgHumidity)
                .Select(s => new AggregatedWeatherData
                {
                    Date = s.Date,
                    AvgHumidity = s.AvgHumidity
                })
                .ToList();
            return sortedData;
        }
        private List<AggregatedWeatherData> AverageTemperature(List<AggregatedWeatherData> dataByDate)
        {
            var sortedData = dataByDate
                .OrderByDescending(o => o.AvgTemperature)
                .Select(s => new AggregatedWeatherData
                {
                    Date = s.Date,
                    AvgTemperature = s.AvgTemperature
                })
                .ToList();
            return sortedData;
        }

    }
}
