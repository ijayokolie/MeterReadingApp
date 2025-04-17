using CsvHelper;
using MeterReadingAPI.Data;
using MeterReadingAPI.DTOs;
using MeterReadingAPI.Model;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MeterReadingAPI.Services
{
    public class MeterReadingService: IMeterReadingService
    {
        private readonly CustomerMeterReadingContext _context;

        public MeterReadingService(CustomerMeterReadingContext context)
        {
            _context = context;
        }

        public async Task<MeterReadingUploadResultDto> ProcessCsvAsync(Stream fileStream)
        {
            var validReadings = new List<MeterReading>();
            var failedCount = 0;
            var successCount = 0;

            using var reader = new StreamReader(fileStream);
            using var csv = new CsvReader(reader, CultureInfo.GetCultureInfo("en-GB"));

            csv.Context.RegisterClassMap<MeterReadingMap>();
            
            while (csv.Read())
            {
                try
                {
                    var reading = csv.GetRecord<MeterReading>();

                    var accountId = reading.AccountId;
                    var dateTime = reading.MeterReadingDateTime;
                    var meterValue = reading.MeterReadValue?.Trim();

                    if (!Regex.IsMatch(meterValue ?? "", @"^\d{5}$"))
                        throw new Exception("Invalid meter read format.");

                    var account = await _context.Accounts.FindAsync(accountId);
                    if (account == null)
                        throw new Exception("Account not found.");

                    var existing = _context.MeterReadings
                        .Where(r => r.AccountId == accountId)
                        .OrderByDescending(r => r.MeterReadingDateTime)
                        .FirstOrDefault();

                    var pending = validReadings
                    .Where(r => r.AccountId == accountId)
                    .OrderByDescending(r => r.MeterReadingDateTime)
                    .FirstOrDefault();

                    if (pending != null && (existing == null || pending.MeterReadingDateTime > existing.MeterReadingDateTime))
                    {
                        existing = pending;
                    }

                    if (existing != null && dateTime < existing.MeterReadingDateTime)
                        throw new Exception("Reading is older than existing.");

                    if (_context.MeterReadings.Any(r =>
                        r.AccountId == accountId &&
                        r.MeterReadingDateTime == dateTime &&
                        r.MeterReadValue == meterValue)
                        || validReadings.Any(r =>
                        r.AccountId == accountId &&
                        r.MeterReadingDateTime == dateTime &&
                        r.MeterReadValue == meterValue))
                        throw new Exception("Duplicate reading.");

                    validReadings.Add(new MeterReading
                    {
                        AccountId = accountId,
                        MeterReadingDateTime = dateTime,
                        MeterReadValue = meterValue
                    });

                    successCount++;
                }
                catch(Exception ex) 
                {
                    failedCount++;
                }
            }

            _context.MeterReadings.AddRange(validReadings);
            _context.SaveChanges();

            return new MeterReadingUploadResultDto
            {
                SuccessfulReadings = successCount,
                FailedReadings = failedCount
            };
        }
    }

}
