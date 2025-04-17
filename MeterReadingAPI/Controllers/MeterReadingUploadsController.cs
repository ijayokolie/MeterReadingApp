using MeterReadingAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace MeterReadingAPI.Controllers;

[ApiController]
[Route("meter-reading-uploads")]
public class MeterReadingUploadsController : ControllerBase
{

    private readonly ILogger<MeterReadingUploadsController> _logger;
    private readonly IMeterReadingService _service;

    public MeterReadingUploadsController(IMeterReadingService service, ILogger<MeterReadingUploadsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    
    [HttpPost]
    public async Task<IActionResult> UploadAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File not provided.");

        string message = $"processing {file.FileName}";
        _logger.LogInformation(message);

        var result = await _service.ProcessCsvAsync(file.OpenReadStream());
        return Ok(result);
    }

}
