
using ClosedXML.Excel;
using CsvHelper;
using System.Globalization;
using SkiaSharp;
using Svg.Skia;

namespace YieldCurveAPI.Services
{
    public class FileGenerationService
    {
        private readonly YieldDataService _yieldDataService;
        private readonly ChartGenerationService _chartGenerationService;

        public FileGenerationService(YieldDataService yieldDataService,
                                    ChartGenerationService chartGenerationService)
        {
            _yieldDataService = yieldDataService;
            _chartGenerationService = chartGenerationService;
        }

        public async Task<byte[]> GenerateCsvFileAsync(DateTime date)
        {
            var yieldData = await _yieldDataService.GetCompleteYieldDataAsync(date);
            if (yieldData == null) return null;

            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            // Write date at the top
            csv.WriteField($"Date: {date:yyyy-MM-dd}");
            csv.NextRecord();

            // Header row
            csv.WriteField("Period (Months)");
            csv.WriteField("Date");
            csv.WriteField("Par");
            csv.WriteField("Zero");
            csv.NextRecord();

            // Get period dates
            var periodDates = new Dictionary<int, DateTime>();
            DateTime startDate = date;
            for (int i = 1; i <= 360; i++)
            {
                periodDates[i] = startDate.AddMonths(i - 1);
            }
            var parYieldCurve = yieldData["ParYieldCurve"];
            var zeroRateCurve = yieldData["ZeroRateCurve"];

            foreach (var period in parYieldCurve.Keys)
            {
                csv.WriteField(period); // Period in months
                csv.WriteField(periodDates[period]); // Period date
                csv.WriteField(parYieldCurve[period]); // Par yield rate
                csv.WriteField(zeroRateCurve[period]); // Zero rate curve
                csv.NextRecord();
            }

            writer.Flush();
            return memoryStream.ToArray();
        }

        public async Task<byte[]> GenerateExcelFileAsync(DateTime date)
        {
            var yieldData = await _yieldDataService.GetCompleteYieldDataAsync(date);
            if (yieldData == null) return null;

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Yield Curve");

            worksheet.Cell(1, 1).Value = "Date:";
            worksheet.Cell(1, 2).Value = date.ToString("yyyy-MM-dd");

            worksheet.Cell(3, 1).Value = "Period (Months)";
            worksheet.Cell(3, 2).Value = "Date";
            worksheet.Cell(3, 3).Value = "Par";
            worksheet.Cell(3, 4).Value = "Zero";

            // Obtain period dates using startDate and adding 359 months
            var periodDates = new Dictionary<int, DateTime>();
            DateTime startDate = date;
            for (int i = 1; i <= 360; i++)
            {
                periodDates[i] = startDate.AddMonths(i - 1);
            }
            var parYieldCurve = yieldData["ParYieldCurve"];
            var zeroRateCurve = yieldData["ZeroRateCurve"];

            int row = 4;
            foreach (var period in zeroRateCurve.Keys)
            {
                worksheet.Cell(row, 1).Value = period;
                worksheet.Cell(row, 2).Value = periodDates[period];
                if (parYieldCurve.ContainsKey(period))
                {
                    worksheet.Cell(row, 3).Value = parYieldCurve[period];
                }
                worksheet.Cell(row, 4).Value = zeroRateCurve[period];
                row++;
            }

            worksheet.Columns().AdjustToContents();

            // Generate and embed chart properly
            var svgData = _chartGenerationService.GenerateYieldCurveChartSvg(yieldData);
            if (!string.IsNullOrEmpty(svgData))
            {
                using var svgStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(svgData));
                var skSvg = new SKSvg();
                if (skSvg.Load(svgStream) != null)
                {
                    using var bitmap = new SKBitmap(1000, 600);
                    using var canvas = new SKCanvas(bitmap);
                    canvas.Clear(SKColors.White);
                    canvas.DrawPicture(skSvg.Picture);

                    using var image = SKImage.FromBitmap(bitmap);
                    using var data = image.Encode(SKEncodedImageFormat.Png, 100);
                    using var imgStream = new MemoryStream(data.ToArray());

                    var picture = worksheet.AddPicture(imgStream).MoveTo(worksheet.Cell(3, 7));
                }
            }

            using var memoryStream = new MemoryStream();
            workbook.SaveAs(memoryStream);
            return memoryStream.ToArray();
        }
    }
}
