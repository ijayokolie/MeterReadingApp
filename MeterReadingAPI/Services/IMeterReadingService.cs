using MeterReadingAPI.DTOs;

namespace MeterReadingAPI.Services
{
    public interface IMeterReadingService
    {
        Task<MeterReadingUploadResultDto> ProcessCsvAsync(Stream fileStream);
    }
}
