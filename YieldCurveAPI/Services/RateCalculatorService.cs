namespace YieldCurveAPI.Services
{
    public class RateCalculatorService
    {
        // parYields: Dictionary<int, double> where key is maturity in months 
        // Returns: Dictionary<int, double> where key is maturity in months for continuous monthly zero rates for 30 years
        public Dictionary<int, double> CalculateContinuousZeroRates(Dictionary<int, double> parYields)
        {
            var zeroRatesAtKeyMaturities = BootstrapZeroRates(parYields);
            var continuousZeroRates = InterpolateZeroRates(zeroRatesAtKeyMaturities);
            return continuousZeroRates; // Return the full continuous set
        }

        private Dictionary<int, double> BootstrapZeroRates(Dictionary<int, double> parYields)
        {
        var zeroRates = new Dictionary<int, double>();
        var sortedMaturities = parYields.Keys.OrderBy(k => k).ToList();

        foreach (var maturity in sortedMaturities)
        {
            double parYield = parYields[maturity] / 100.0;
            double maturityInYears = maturity / 12.0;

            if (maturity <= 12) // For maturities <= 12 months, use the par rate as the zero rate?
            {
                zeroRates[maturity] = parYield * 100.0;
                continue;
            }

            // Sum the discounted coupons for prev maturities
            double sumDiscountedCoupons = 0.0;
            for (int year = 1; year < maturityInYears; year++)
            {
                int prevMaturityMonths = year * 12;
                if (zeroRates.ContainsKey(prevMaturityMonths))
                {
                    double prevZeroRate = zeroRates[prevMaturityMonths] / 100.0;
                    double discountFactor = 1.0 / Math.Pow(1.0 + prevZeroRate, year);
                    sumDiscountedCoupons += parYield * discountFactor;
                }
            }

            // Solve for zero rate at current maturity
            double remainingValue = 1.0 - sumDiscountedCoupons; // principal amount
            double zeroRate = Math.Pow(1.0 / remainingValue, 1.0 / maturityInYears) - 1.0; // ANNUAL compounding
            zeroRates[maturity] = zeroRate * 100.0;
        }

        return zeroRates;
    }

        private Dictionary<int, double> InterpolateZeroRates(Dictionary<int, double> zeroRatesAtKeyMaturities)
        {
            var continuousZeroRates = new Dictionary<int, double>();
            var sortedMaturities = zeroRatesAtKeyMaturities.Keys.OrderBy(k => k).ToList();

            // Known zero rates
            foreach (var maturity in sortedMaturities)
            {
                continuousZeroRates[maturity] = zeroRatesAtKeyMaturities[maturity];
            }

            // Interpolate up to 360 months
            for (int month = 1; month <= 360; month++)
            {
                if (!continuousZeroRates.ContainsKey(month))
                {
                    // Find the previous and next known maturities
                    int prevMaturity = sortedMaturities.Where(m => m < month).Max();
                    int nextMaturity = sortedMaturities.Where(m => m > month).Min();

                    double prevRate = zeroRatesAtKeyMaturities[prevMaturity];
                    double nextRate = zeroRatesAtKeyMaturities[nextMaturity];

                    // Straight line linear interpolation
                    double slope = (nextRate - prevRate) / (nextMaturity - prevMaturity);
                    double interpolatedRate = prevRate + slope * (month - prevMaturity);
                    continuousZeroRates[month] = interpolatedRate;
                }
            }

            return continuousZeroRates.OrderBy(k => k.Key).ToDictionary(k => k.Key, v => v.Value);
        }
    }
}