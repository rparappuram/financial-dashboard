import { type NextRequest, NextResponse } from "next/server";

export async function GET(request: NextRequest) {
  const searchParams = request.nextUrl.searchParams;
  const date = searchParams.get("date") || "latest";
  const type = searchParams.get("type") || "csv";

  // In a real app, this would fetch actual data from a database or external API
  // For demonstration, we're generating placeholder data

  // Generate file content based on type
  let content: string;
  let contentType: string;
  let filename: string;

  if (type === "xlsx") {
    content = generateExcelContent(date);
    contentType = "application/vnd.ms-excel";
    filename = `treasury-data-${date}.xlsx`;
  } else {
    content = generateCSVContent(date);
    contentType = "text/csv";
    filename = `treasury-data-${date}.csv`;
  }

  // Set headers for file download
  return new NextResponse(content, {
    headers: {
      "Content-Type": contentType,
      "Content-Disposition": `attachment; filename="${filename}"`,
    },
  });
}

// Function to generate CSV content
function generateCSVContent(date: string) {
  const headers = ["Maturity", "Par Yield (%)", "Zero Rate (%)"];
  const maturities = [
    "1M",
    "3M",
    "6M",
    "1Y",
    "2Y",
    "3Y",
    "5Y",
    "7Y",
    "10Y",
    "30Y",
  ];

  // Generate placeholder data
  // In a real app, this would be actual data from a database
  const rows = maturities.map((maturity, index) => {
    // Simple placeholder values that would be replaced with real data
    const parYield = (0.5 + index * 0.3).toFixed(3);
    const zeroRate = (0.45 + index * 0.3).toFixed(3);
    return `${maturity},${parYield},${zeroRate}`;
  });

  return [headers.join(","), ...rows].join("\n");
}

// Function to generate Excel XML content (simplified for demonstration)
function generateExcelContent(date: string) {
  // This is a simplified Excel XML format for demonstration
  // In a real app, you would use a library like exceljs or xlsx
  let xml = '<?xml version="1.0"?><?mso-application progid="Excel.Sheet"?>';
  xml += '<Workbook xmlns="urn:schemas-microsoft-com:office:spreadsheet">';
  xml += '<Worksheet ss:Name="Treasury Data"><Table>';

  // Headers
  xml += '<Row><Cell><Data ss:Type="String">Maturity</Data></Cell>';
  xml += '<Cell><Data ss:Type="String">Par Yield (%)</Data></Cell>';
  xml += '<Cell><Data ss:Type="String">Zero Rate (%)</Data></Cell></Row>';

  // Data rows
  const maturities = [
    "1M",
    "3M",
    "6M",
    "1Y",
    "2Y",
    "3Y",
    "5Y",
    "7Y",
    "10Y",
    "30Y",
  ];

  maturities.forEach((maturity, index) => {
    // Simple placeholder values that would be replaced with real data
    const parYield = (0.5 + index * 0.3).toFixed(3);
    const zeroRate = (0.45 + index * 0.3).toFixed(3);

    xml += "<Row>";
    xml += `<Cell><Data ss:Type="String">${maturity}</Data></Cell>`;
    xml += `<Cell><Data ss:Type="Number">${parYield}</Data></Cell>`;
    xml += `<Cell><Data ss:Type="Number">${zeroRate}</Data></Cell>`;
    xml += "</Row>";
  });

  xml += "</Table></Worksheet></Workbook>";
  return xml;
}
