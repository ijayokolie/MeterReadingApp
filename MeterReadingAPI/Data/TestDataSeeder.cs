using CsvHelper;
using MeterReadingAPI.Model;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace MeterReadingAPI.Data
{
    public class TestDataSeeder
    {
        private readonly CustomerMeterReadingContext _context;

        public TestDataSeeder(CustomerMeterReadingContext context)
        {
            _context = context;
        }
        public async Task SeedAsync()
        {
            // Apply any pending migrations
            await _context.Database.MigrateAsync();

            if (!_context.Accounts.Any())
            {

                using (var reader = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "Test_Accounts.csv")))
                using (var csv = new CsvReader(reader, CultureInfo.GetCultureInfo("en-GB")))
                {

                    csv.Context.RegisterClassMap<AccountMap>();
                    var accounts = csv.GetRecords<Account>().ToList();
                                      
                    _context.Accounts.AddRange(accounts);
                    _context.SaveChanges();
                }
            }
        }
    }

}
