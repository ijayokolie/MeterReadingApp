using MeterReadingAPI.Data;
using MeterReadingAPI.Model;
using MeterReadingAPI.Services;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace MeterReadingAPITests
{
    public class MeterReadingServiceTests
    {
        private CustomerMeterReadingContext _context;
        private MeterReadingService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CustomerMeterReadingContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _context = new CustomerMeterReadingContext(options);
            _context.Database.EnsureDeleted(); // clean slate
            _context.Database.EnsureCreated();

            // Seed account
            _context.Accounts.AddRange(new List<Account>
            {
                new Account { AccountId = 1234, FirstName = "Test", LastName = "User" },
                new Account { AccountId = 5678, FirstName = "Second", LastName = "User" }
            });
            _context.SaveChanges();

            _service = new MeterReadingService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        private Stream GenerateCsv(string content)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(content));
        }

        [Test]
        public async Task ProcessCsvAsync_ValidAndInvalidRows_ReturnsCorrectCounts()
        {
            // Arrange
            var csv =
@"AccountId,MeterReadingDateTime,MeterReadValue
1234,22/04/2019 09:24,12345
1234,22/04/2019 09:24,12345 
5678,22/04/2019 09:24,abcde 
9999,22/04/2019 09:24,12345 
1234,21/04/2019 09:24,12346"; 

            var stream = GenerateCsv(csv);

            // Act
            var result = await _service.ProcessCsvAsync(stream);

            // Assert
            Assert.AreEqual(1, result.SuccessfulReadings);
            Assert.AreEqual(4, result.FailedReadings);
        }

        [Test]
        public async Task ProcessCsvAsync_AllValid_ReadingsStored()
        {
            var csv =
@"AccountId,MeterReadingDateTime,MeterReadValue
1234,22/04/2019 09:24,12345
5678,22/04/2019 09:25,54321";

            var stream = GenerateCsv(csv);

            var result = await _service.ProcessCsvAsync(stream);

            Assert.AreEqual(2, result.SuccessfulReadings);
            Assert.AreEqual(0, result.FailedReadings);

            Assert.AreEqual(2, await _context.MeterReadings.CountAsync());
        }
    }
}