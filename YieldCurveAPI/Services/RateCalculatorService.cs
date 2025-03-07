namespace YieldCurveAPI.Services
{
    public class RateCalculatorService
    {
        /// <summary>
        /// Calculates continuous monthly zero rates for up to 30 years (360 months) based on provided par yields.
        /// </summary>
        /// <param name="parYields">Dictionary where key is maturity in months and value is the par yield in percentage (e.g., 4.44 for 4.44%).</param>
        /// <returns>Dictionary where key is maturity in months (1 to 360) and value is the continuous zero rate in percentage.</returns>
        public Dictionary<int, double> CalculateContinuousZeroRates(Dictionary<int, double> parYields)
        {
            // Step 1: Sort the key maturities from the par yields dictionary
            var keyMaturities = parYields.Keys.OrderBy(k => k).ToList();
            var discountFactors = new Dictionary<int, double>(); // Store discount factors for key maturities
            var zeroRates = new Dictionary<int, double>();      // Store zero rates

            // Step 2: Bootstrap zero rates for each key maturity
            foreach (var T in keyMaturities)
            {
                double yield = parYields[T] / 100; // Convert yield from percentage to decimal

                if (T <= 6)
                {
                    // For maturities ≤ 6 months, assume a single payment at maturity
                    // Discount factor: DF_T = 1 / (1 + yield * (T/12))
                    double DF_T = 1 / (1 + yield * (T / 12.0));
                    double r_T = Math.Pow(1 / DF_T, 1.0 / T) - 1; // Zero rate: (1 / DF_T)^(1/T) - 1
                    double annualized_r_T = Math.Pow(1 + r_T, 12.0) - 1; // Annualize the rate
                    zeroRates[T] = annualized_r_T * 100; // Store in percentage
                    discountFactors[T] = DF_T;
                }
                else
                {
                    // For maturities > 6 months, assume semi-annual coupons
                    double coupon = yield / 2; // Semi-annual coupon payment
                    List<int> paymentTimes = new List<int>();
                    for (int k = 6; k <= T; k += 6)
                    {
                        paymentTimes.Add(k); // Coupon payments every 6 months
                    }

                    // Define the PV function to solve for r_T where PV = 1
                    Func<double, double> PV_minus_1 = (r_T) =>
                    {
                        double DF_T = 1 / Math.Pow(1 + r_T, T); // DF at maturity T
                        double PV = 0;
                        foreach (var t in paymentTimes)
                        {
                            double cashFlow = (t == T) ? (1 + coupon) : coupon; // Principal + coupon at maturity, else coupon only
                            double DF_t;

                            if (discountFactors.ContainsKey(t))
                            {
                                DF_t = discountFactors[t]; // Use known discount factor
                            }
                            else
                            {
                                // Interpolate ln(DF_t) between the last key maturity before t and T
                                var T1 = keyMaturities.Last(k => k < t);
                                var T2 = T; // T is the current key maturity being solved // TODO: maybe change to T2 = keyMaturities.First(k => k > t);
                                double ln_DF_T1 = Math.Log(discountFactors[T1]);
                                double ln_DF_T2 = Math.Log(DF_T);
                                double ln_DF_t = ln_DF_T1 + ((double)(t - T1) / (T2 - T1)) * (ln_DF_T2 - ln_DF_T1);
                                DF_t = Math.Exp(ln_DF_t);
                            }
                            PV += cashFlow * DF_t;
                        }
                        return PV - 1; // Return PV - 1 to find root
                    };

                    // Use bisection method to solve for r_T
                    double r_min = 0;    // Minimum possible rate
                    double r_max = 1;    // Maximum possible rate (100%)
                    double tolerance = 1e-8;
                    while (r_max - r_min > tolerance)
                    {
                        double r_mid = (r_min + r_max) / 2;
                        double pv_mid = PV_minus_1(r_mid);
                        if (pv_mid > 0)
                            r_min = r_mid; // PV too high, increase rate
                        else
                            r_max = r_mid; // PV too low, decrease rate
                    }
                    double r_T = (r_min + r_max) / 2;
                    double DF_T = 1 / Math.Pow(1 + r_T, T);
                    double annualized_r_T = Math.Pow(1 + r_T, 12.0) - 1; // Annualize the rate
                    zeroRates[T] = annualized_r_T * 100; // Store in percentage
                    discountFactors[T] = DF_T;
                }
            }

            // Step 3: Interpolate zero rates for all months from 1 to 360
            for (int t = 1; t <= 360; t++)
            {
                if (!zeroRates.ContainsKey(t))
                {
                    // Find surrounding key maturities
                    var T1 = keyMaturities.Last(k => k < t);
                    var T2 = keyMaturities.First(k => k > t);
                    double ln_DF_T1 = Math.Log(discountFactors[T1]);
                    double ln_DF_T2 = Math.Log(discountFactors[T2]);
                    double ln_DF_t = ln_DF_T1 + ((double)(ln_DF_T2 - ln_DF_T1) / (T2 - T1)) * (t - T1); // Equation obtained from Excel
                    double DF_t = Math.Exp(ln_DF_t);
                    double r_t = Math.Pow(1 / DF_t, 1.0 / t) - 1;
                    double annualized_r_t = Math.Pow(1 + r_t, 12.0) - 1;
                    zeroRates[t] = annualized_r_t * 100; // Store in percentage
                }
            }
            return zeroRates.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}