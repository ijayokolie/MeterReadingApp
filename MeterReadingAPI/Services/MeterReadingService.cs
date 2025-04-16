using CsvHelper;
using MeterReadingAPI.Data;
using MeterReadingAPI.DTOs;
using MeterReadingAPI.Model;
using System.Globalization;

namespace MeterReadingAPI.Services
{
    public class MeterReadingService
    {
        private readonly CustomerMeterReadingContext _context;

        public MeterReadingService(CustomerMeterReadingContext context)
        {
            _context = context;
        }

        public MeterReadingUploadResultDto ProcessCsv(Stream fileStream)
        {
            int success = 0, failure = 0;


            using (var reader = new StreamReader(fileStream))
            using (var csv = new CsvReader(reader, CultureInfo.GetCultureInfo("en-GB")))
            {
               

                csv.Context.RegisterClassMap<MeterReadingMap>();
              

                while (csv.Read())
                {
                    var reading = csv.GetRecord<MeterReading>();
                   
                    Console.WriteLine($"{reading.AccountId}: {reading.MeterReadingDateTime} : {reading.MeterReadValue}");
                    success++;
                }
            }
                     
           
        //    _context.SaveChanges();

            return new MeterReadingUploadResultDto
            {
                SuccessfulReadings = success,
                FailedReadings = failure
            };
        }
    }

}
