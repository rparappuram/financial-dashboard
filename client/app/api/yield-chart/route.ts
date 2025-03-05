import { type NextRequest, NextResponse } from "next/server"

export async function GET(request: NextRequest) {
  const searchParams = request.nextUrl.searchParams
  const date = searchParams.get("date") || "latest"

  // In a real application, this would fetch the actual SVG from a data source
  // For demonstration, we're returning a placeholder SVG
  const svgContent = generatePlaceholderSVG(date)

  return new NextResponse(svgContent, {
    headers: {
      "Content-Type": "image/svg+xml",
    },
  })
}

// Function to generate a placeholder SVG chart
function generatePlaceholderSVG(date: string) {
  // This is a simple placeholder SVG that would be replaced with actual chart SVG from backend
  return `
    <svg width="100%" height="100%" viewBox="0 0 800 400" xmlns="http://www.w3.org/2000/svg">
      <style>
        .title { font: bold 20px sans-serif; fill: #333; }
        .axis { font: 12px sans-serif; }
        .axis-label { font: 14px sans-serif; fill: #555; }
        .line-par { fill: none; stroke: #1f77b4; stroke-width: 2; }
        .line-zero { fill: none; stroke: #ff7f0e; stroke-width: 2; }
        .dot { stroke-width: 1; stroke: white; }
        .dot-par { fill: #1f77b4; }
        .dot-zero { fill: #ff7f0e; }
        .legend { font: 14px sans-serif; }
        .grid { stroke: #e0e0e0; stroke-width: 1; }
        .date-label { font: italic 14px sans-serif; fill: #777; }
      </style>
      
      <!-- Title -->
      <text x="400" y="30" text-anchor="middle" class="title">U.S. Treasury Yield Curve</text>
      <text x="400" y="50" text-anchor="middle" class="date-label">
        ${date === "latest" ? "Latest Available Data" : `Data for ${date}`}
      </text>
      
      <!-- Axes -->
      <line x1="50" y1="350" x2="750" y2="350" stroke="#333" />
      <line x1="50" y1="50" x2="50" y2="350" stroke="#333" />
      
      <!-- X-axis labels -->
      <text x="100" y="370" text-anchor="middle" class="axis">1M</text>
      <text x="170" y="370" text-anchor="middle" class="axis">3M</text>
      <text x="240" y="370" text-anchor="middle" class="axis">6M</text>
      <text x="310" y="370" text-anchor="middle" class="axis">1Y</text>
      <text x="380" y="370" text-anchor="middle" class="axis">2Y</text>
      <text x="450" y="370" text-anchor="middle" class="axis">3Y</text>
      <text x="520" y="370" text-anchor="middle" class="axis">5Y</text>
      <text x="590" y="370" text-anchor="middle" class="axis">7Y</text>
      <text x="660" y="370" text-anchor="middle" class="axis">10Y</text>
      <text x="730" y="370" text-anchor="middle" class="axis">30Y</text>
      
      <!-- Y-axis labels -->
      <text x="40" y="350" text-anchor="end" class="axis">0%</text>
      <text x="40" y="300" text-anchor="end" class="axis">1%</text>
      <text x="40" y="250" text-anchor="end" class="axis">2%</text>
      <text x="40" y="200" text-anchor="end" class="axis">3%</text>
      <text x="40" y="150" text-anchor="end" class="axis">4%</text>
      <text x="40" y="100" text-anchor="end" class="axis">5%</text>
      <text x="40" y="50" text-anchor="end" class="axis">6%</text>
      
      <!-- Grid lines -->
      <line x1="50" y1="300" x2="750" y2="300" class="grid" />
      <line x1="50" y1="250" x2="750" y2="250" class="grid" />
      <line x1="50" y1="200" x2="750" y2="200" class="grid" />
      <line x1="50" y1="150" x2="750" y2="150" class="grid" />
      <line x1="50" y1="100" x2="750" y2="100" class="grid" />
      <line x1="50" y1="50" x2="750" y2="50" class="grid" />
      
      <!-- Axis labels -->
      <text x="400" y="395" text-anchor="middle" class="axis-label">Maturity</text>
      <text x="15" y="200" text-anchor="middle" transform="rotate(-90, 15, 200)" class="axis-label">Yield (%)</text>
      
      <!-- Par Yield Curve -->
      <path d="M100,320 L170,310 L240,300 L310,280 L380,250 L450,230 L520,210 L590,200 L660,195 L730,190" class="line-par" />
      
      <!-- Zero Rate Curve -->
      <path d="M100,325 L170,315 L240,305 L310,285 L380,255 L450,235 L520,215 L590,205 L660,200 L730,195" class="line-zero" />
      
      <!-- Data points -->
      <circle cx="100" cy="320" r="4" class="dot dot-par" />
      <circle cx="170" cy="310" r="4" class="dot dot-par" />
      <circle cx="240" cy="300" r="4" class="dot dot-par" />
      <circle cx="310" cy="280" r="4" class="dot dot-par" />
      <circle cx="380" cy="250" r="4" class="dot dot-par" />
      <circle cx="450" cy="230" r="4" class="dot dot-par" />
      <circle cx="520" cy="210" r="4" class="dot dot-par" />
      <circle cx="590" cy="200" r="4" class="dot dot-par" />
      <circle cx="660" cy="195" r="4" class="dot dot-par" />
      <circle cx="730" cy="190" r="4" class="dot dot-par" />
      
      <circle cx="100" cy="325" r="4" class="dot dot-zero" />
      <circle cx="170" cy="315" r="4" class="dot dot-zero" />
      <circle cx="240" cy="305" r="4" class="dot dot-zero" />
      <circle cx="310" cy="285" r="4" class="dot dot-zero" />
      <circle cx="380" cy="255" r="4" class="dot dot-zero" />
      <circle cx="450" cy="235" r="4" class="dot dot-zero" />
      <circle cx="520" cy="215" r="4" class="dot dot-zero" />
      <circle cx="590" cy="205" r="4" class="dot dot-zero" />
      <circle cx="660" cy="200" r="4" class="dot dot-zero" />
      <circle cx="730" cy="195" r="4" class="dot dot-zero" />
      
      <!-- Legend -->
      <rect x="600" y="70" width="12" height="12" fill="#1f77b4" />
      <text x="620" y="80" class="legend">Par Yield</text>
      <rect x="600" y="90" width="12" height="12" fill="#ff7f0e" />
      <text x="620" y="100" class="legend">Zero Rate</text>
    </svg>
  `
}

