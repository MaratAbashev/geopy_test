using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeopyTestConsoleApp.DataModels;
using GeopyTestConsoleApp.DataModels.Database;
using LinqToDB;
using LinqToDB.Async;
using LinqToDB.Data;

namespace GeopyTestConsoleApp
{
    class Program
    {
        public static async Task Main()
        {
            await using var db = new GeopyDb(new DataOptions().WithOptions(
                new ConnectionOptions()
                    .WithProviderName(ProviderName.PostgreSQL)
                    .WithConnectionString(
                        "Host=localhost;Port=5433;Database=geopy_db;Username=postgres;Password=postgres;")));
            Console.WriteLine("Нажмите Enter если хотите заполнить бд данными");
            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.Enter)
                await FillData(db);
            
            Console.WriteLine("\nВведите дату замера в формате YYYY-MM-DD");
            bool hasDateTime = DateTime.TryParse(Console.ReadLine(), out DateTime date);

            var query = db.Wells
                .InnerJoin(db.Divisions, 
                    (well, division) => division.Id == well.DivisionId, 
                    (well, division) => new
                    {
                        DivisionName = division.Name, 
                        Well = well
                    })
                .InnerJoin(db.Measurements,
                    (well, measurement) => well.Well.Id == measurement.WellId,
                    (well, measurement) 
                        => new
                        {
                            well.DivisionName, 
                            WellName = well.Well.Name, 
                            Measurement = measurement
                        });
                if (hasDateTime)
                    query = query.Where(measurement => measurement.Measurement.MeasurementTime.Date == date.Date);
                var resultList = await query.GroupBy(measurement => new
                {
                    measurement.DivisionName, 
                    measurement.WellName, 
                    measurement.Measurement.MeasurementType,
                    measurement.Measurement.MeasurementTime.Date
                })
                .Select(group => new ResultModel()
                {
                    DivisionName = group.Key.DivisionName,
                    WellName = group.Key.WellName,
                    Date = group.Key.Date,
                    MeasurementType = group.Key.MeasurementType,
                    MinValue = group.Min(selector => selector.Measurement.MeasurementValue),
                    MaxValue = group.Max(selector => selector.Measurement.MeasurementValue),
                    Count = group.Count()
                })
                .ToListAsync();
            SaveToCsv(resultList, "measurements.csv");
        }
        
        public static void SaveToCsv(List<ResultModel> data, string filePath)
        {
            using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
            
            writer.WriteLine("Подразделение;Название скважины;Дата замера;Тип замера;Минимальное значение замера;Максимальное значение замера;Количество замеров");

            foreach (var row in data)
            {
                string Escape(string s) => $"\"{s.Replace("\"", "\"\"")}\"";

                var line = $"{Escape(row.DivisionName)};" +
                           $"{Escape(row.WellName)};" +
                           $"{row.Date:yyyy-MM-dd};" +
                           $"{row.MeasurementType};" +
                           $"{row.MinValue};" +
                           $"{row.MaxValue};" +
                           $"{row.Count}";

                writer.WriteLine(line);
            }
        }

        private static async Task FillData(GeopyDb db)
        {
            var measurementList = new List<Measurement>();
            var rnd = new Random();
            for (int divisionsCount = 0; divisionsCount < 3; divisionsCount++)
            {
                var division = new Division()
                {
                    Name = $"Division {divisionsCount}"
                };
                division.Id = Convert.ToInt32(await db.InsertWithIdentityAsync(division));
                
                for (int wellsCount = 0; wellsCount < 5; wellsCount++)
                {
                    var well = new Well()
                    {
                        Name = $"Well {wellsCount}d{divisionsCount}",
                        Division = division,
                        CommissioningDate = DateTime.Now.AddYears(-1),
                        DivisionId = division.Id,
                    };
                    well.Id = Convert.ToInt32(await db.InsertWithIdentityAsync(well));
                    
                    for (int measurementsCount = 0; measurementsCount < 3 * 5 * 100; measurementsCount++)
                    {
                        var measurement = new Measurement()
                        {
                            MeasurementType = (MeasurementType)(measurementsCount % 3),
                            MeasurementTime = DateTime.Today.AddMonths(-1).AddDays(measurementsCount % 5).AddMinutes(measurementsCount % 100),
                            MeasurementValue = (decimal)(rnd.Next(100, 300) + rnd.NextDouble()),
                            Well = well,
                            WellId = well.Id
                        };
                        measurementList.Add(measurement);
                    }
                }
            }
            await db.BulkCopyAsync(measurementList);
        }
    }
}