using CsvHelper.Configuration;

namespace MeterReadingAPI.Model
{
   public class MeterReading
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public DateTime MeterReadingDateTime { get; set; }
        public string MeterReadValue { get; set; }

        public Account Account { get; set; }
    }

    public class Account
    {
        public int AccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public IList<MeterReading> MeterReadings { get; set; }
    }

    public class MeterReadingMap : ClassMap<MeterReading>
    {
        public MeterReadingMap()
        {
            Map(m => m.AccountId).Index(0);
            Map(m => m.MeterReadingDateTime).Index(1);
            Map(m => m.MeterReadValue).Index(2);
        }
    }

    public class AccountMap : ClassMap<Account>
    {
        public AccountMap()
        {
            Map(a => a.AccountId).Index(0);
            Map(a => a.FirstName).Index(1);
            Map(a => a.LastName).Index(2);
        }
    }




}
