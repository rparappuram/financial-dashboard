using Microsoft.AspNetCore.Mvc;
using YieldCurveAPI.Services;

[Route("api/yield-chart")]
[ApiController]
public class YieldChartController : ControllerBase
{
    private readonly YieldDataService _yieldDataService;
    private readonly ChartGenerationService _chartGenerationService;

    public YieldChartController(YieldDataService yieldDataService, ChartGenerationService chartGenerationService)
    {
        _yieldDataService = yieldDataService;
        _chartGenerationService = chartGenerationService;
    }

    // GET /api/yield-chart?date=YYYY-MM-DD
    [HttpGet]
    public async Task<IActionResult> GetYieldChart([FromQuery] string? date = null)
    {
        DateTime requestedDate;

        // If no date is provided, get the latest available date
        if (string.IsNullOrEmpty(date))
        {
            requestedDate = DateTime.Today;
            var data = await _yieldDataService.GetCompleteYieldDataAsync(requestedDate);
            while (data == null)
            {
                requestedDate = requestedDate.AddDays(-1);
                data = await _yieldDataService.GetCompleteYieldDataAsync(requestedDate);
            }
        }
        else
        {
            // If a date is provided, parse it and fetch the data
            if (!DateTime.TryParse(date, out requestedDate))
            {
                return BadRequest("Invalid date format. Please use YYYY-MM-DD.");
            }
            var data = await _yieldDataService.GetCompleteYieldDataAsync(requestedDate);
            if (data == null)
            {
                return NotFound($"No yield curve data available for {requestedDate:yyyy-MM-dd}.");
            }
            // return Ok(data);
        }

        // Generate the yield curve chart as an SVG
        var svgChart = _chartGenerationService.GenerateYieldCurveChartSvg(await _yieldDataService.GetCompleteYieldDataAsync(requestedDate));

        return Content(svgChart, "image/svg+xml");
    }
}