using CsvHelper;
using MeterReadingAPI.DTOs;
using MeterReadingAPI.Model;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace MeterReadingAPI.Controllers;

[ApiController]
[Route("meter-reading-uploads")]
public class MeterReadingUploadsController : ControllerBase
{

    private readonly ILogger<MeterReadingUploadsController> _logger;
    //private readonly MeterReadingService _service;

    public MeterReadingUploadsController(ILogger<MeterReadingUploadsController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public IActionResult Post(IFormFile file) 
    {
        if (file == null || file.Length == 0)
            return BadRequest("File not provided.");
        int success = 0, failure = 0;


        using (var reader = new StreamReader(file.OpenReadStream()))
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

        var response = new MeterReadingUploadResultDto
        {
            SuccessfulReadings = success,
            FailedReadings = failure
        };
        return Ok(response);
    }

}
