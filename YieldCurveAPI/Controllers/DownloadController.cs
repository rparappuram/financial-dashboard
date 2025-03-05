using Microsoft.AspNetCore.Mvc;
using YieldCurveAPI.Services;

namespace YieldCurveAPI.Controllers
{
    [Route("api/download")]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        private readonly FileGenerationService _fileGenerationService;
        private readonly YieldDataService _yieldDataService;

        public DownloadController(FileGenerationService fileGenerationService, YieldDataService yieldDataService)
        {
            _fileGenerationService = fileGenerationService;
            _yieldDataService = yieldDataService;
        }

        // GET /api/download?date=YYYY-MM-DD&type=csv|xlsx
        [HttpGet]
        public async Task<IActionResult> Download([FromQuery] string? date, [FromQuery] string type = "csv")
        {
            // 1. Parse or find the date
            //    If no date is provided, fallback to the latest date for which data is available.
            DateTime requestedDate;
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
                if (!DateTime.TryParse(date, out requestedDate))
                {
                    return BadRequest("Invalid date format. Please use YYYY-MM-DD.");
                }
                // Confirm data is available
                var checkData = await _yieldDataService.GetCompleteYieldDataAsync(requestedDate);
                if (checkData == null)
                {
                    return NotFound($"No yield data available for {requestedDate:yyyy-MM-dd}");
                }
            }

            // 2. Generate the file (CSV or Excel) with the requested date's data.
            byte[] fileBytes;
            string fileName;
            string contentType;

            switch (type.ToLower())
            {
                case "csv":
                    fileBytes = await _fileGenerationService.GenerateCsvFileAsync(requestedDate);
                    fileName = $"yield-data-{requestedDate:yyyy-MM-dd}.csv";
                    contentType = "text/csv";
                    break;
                case "xls":
                case "xlsx":
                case "excel":
                    fileBytes = await _fileGenerationService.GenerateExcelFileAsync(requestedDate);
                    fileName = $"yield-data-{requestedDate:yyyy-MM-dd}.xlsx";
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;
                default:
                    return BadRequest("Invalid file type. Please specify 'csv' or 'xlsx'.");
            }

            if (fileBytes == null || fileBytes.Length == 0)
            {
                return NotFound($"No file data available for {requestedDate:yyyy-MM-dd}");
            }

            // 3. Return the file to the client
            return File(fileBytes, contentType, fileName);
        }
    }
}