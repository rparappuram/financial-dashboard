using System.Xml.Linq;

namespace YieldCurveAPI.Services;

public class TreasuryXmlService
{
    private readonly HttpClient _httpClient;
    private const string TreasuryUrl = "https://home.treasury.gov/resource-center/data-chart-center/interest-rates/pages/xmlview?data=daily_treasury_yield_curve&field_tdr_date_value=";

    public TreasuryXmlService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Dictionary<int, double>> GetYieldCurveDataAsync(DateTime requestedDate)
    {
        string year = requestedDate.Year.ToString();
        string url = $"{TreasuryUrl}{year}";

        try
        {
            // Fetch XML from Treasury API
            string xmlData = await _httpClient.GetStringAsync(url);
            XDocument xmlDoc = XDocument.Parse(xmlData);

            // Extract all entries
            var entries = xmlDoc.Descendants("{http://www.w3.org/2005/Atom}entry");


            // Extract relevant entries for the requested date
            var entry = entries.FirstOrDefault(e =>
            {
                var dateElement = e.Descendants("{http://schemas.microsoft.com/ado/2007/08/dataservices}NEW_DATE").FirstOrDefault();
                if (dateElement == null) return false;

                if (DateTime.TryParse(dateElement.Value, out var entryDate))
                {
                    return entryDate.Date == requestedDate.Date; // Ensure exact match
                }
                return false;
            });

            if (entry == null)
            {
                return null; // No data available for this date
            }

            // Parse yield curve rates
            var properties = entry.Descendants("{http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}properties").FirstOrDefault();
            var yieldCurveData = new Dictionary<int, double>();

            if (properties != null)
            {
                var requiredKeys = new Dictionary<string, int>
                {
                    { "BC_1MONTH", 1 },
                    { "BC_2MONTH", 2 },
                    { "BC_3MONTH", 3 },
                    { "BC_4MONTH", 4 },
                    { "BC_6MONTH", 6 },
                    { "BC_1YEAR", 12 },
                    { "BC_2YEAR", 24 },
                    { "BC_3YEAR", 36 },
                    { "BC_5YEAR", 60 },
                    { "BC_7YEAR", 84 },
                    { "BC_10YEAR", 120 },
                    { "BC_20YEAR", 240 },
                    { "BC_30YEAR", 360 }
                };

                foreach (var element in properties.Elements())
                {
                    string key = element.Name.LocalName;
                    if (requiredKeys.ContainsKey(key) && double.TryParse(element.Value, out double yield))
                    {
                        yieldCurveData[requiredKeys[key]] = yield;
                    }
                }
            }

            return yieldCurveData;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching/parsing Treasury data: {ex.Message}");
            return null;
        }
    }
}