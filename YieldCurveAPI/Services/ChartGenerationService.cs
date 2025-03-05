using ScottPlot;

namespace YieldCurveAPI.Services
{
    public class ChartGenerationService
    {
        public string GenerateYieldCurveChartSvg(Dictionary<string, Dictionary<int, double>> yieldData, string type = "svg")
        {
            if (yieldData == null || !yieldData.ContainsKey("ParYieldCurve") || !yieldData.ContainsKey("ZeroRateCurve"))
            {
                return "<svg xmlns='http://www.w3.org/2000/svg' width='500' height='300'><text x='10' y='50' font-size='20'>No Data Available</text></svg>";
            }

            var parYields = yieldData["ParYieldCurve"];
            var zeroRates = yieldData["ZeroRateCurve"];

            // Convert dictionary keys to numerical maturities
            double[] maturities = new double[parYields.Count];
            double[] parYieldValues = new double[parYields.Count];
            double[] zeroRateValues = new double[zeroRates.Count];

            int index = 0;
            foreach (var kvp in parYields)
            {
                maturities[index] = kvp.Key; // Maturity in months
                parYieldValues[index] = kvp.Value; // Yield percentage
                index++;
            }

            index = 0;
            foreach (var kvp in zeroRates)
            {
                zeroRateValues[index] = kvp.Value; // Zero rate percentage
                index++;
            }

            // Create ScottPlot figure
            var plt = new ScottPlot.Plot();

            // Add Par Yield Curve (solid line)
            var parPlot = plt.Add.Scatter(maturities, parYieldValues);
            parPlot.LegendText = "Discrete-Tenor Par Curve";
            parPlot.LineWidth = 2;

            // Add Zero Rate Curve (dashed line)
            var zeroPlot = plt.Add.Scatter(maturities, zeroRateValues);
            zeroPlot.LegendText = "Continuous Monthly Zero-Rate Curve";
            zeroPlot.LineWidth = 2;
            zeroPlot.LinePattern = LinePattern.Dashed;

            // Chart Labels
            plt.Title("U.S. Treasury Rates");
            plt.Axes.Bottom.Label.Text = "Future Monthly Periods";
            plt.Axes.Left.Label.Text = "Interest Rate (%)";
            plt.Legend.IsVisible = true;

            // Generate chart
            return plt.GetSvgXml(800, 500);
        }
    }
}