namespace YieldCurveAPI.Services
{
    public class YieldDataService
    {
        private readonly TreasuryXmlService _treasuryXmlService;
        private readonly RateCalculatorService _rateCalculatorService;

        public YieldDataService(TreasuryXmlService treasuryXmlService, RateCalculatorService rateCalculatorService)
        {
            _treasuryXmlService = treasuryXmlService;
            _rateCalculatorService = rateCalculatorService;
        }

        public async Task<Dictionary<string, Dictionary<int, double>>> GetCompleteYieldDataAsync(DateTime date)
        {
            // Fetch par yields from Treasury API
            var parYields = await _treasuryXmlService.GetYieldCurveDataAsync(date);
            if (parYields == null) return null;

            // Calculate continuous zero rates
            var zeroRates = _rateCalculatorService.CalculateContinuousZeroRates(parYields);
            if (zeroRates == null) return null;
                Console.WriteLine($"\n\n\nTotal entries found: \n\n\n");

            // Combine par yields and zero rates into a single dictionary
            var result = new Dictionary<string, Dictionary<int, double>>
            {
                { "ParYieldCurve", parYields },
                { "ZeroRateCurve", zeroRates },
            };

            return result;
        }
    }
}