"use client"

import { useState, useEffect } from "react"
import { isWeekend, isValid, parse } from "date-fns"
import { Download } from "lucide-react"

import { Button } from "@/components/ui/button"
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from "@/components/ui/dropdown-menu"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Skeleton } from "@/components/ui/skeleton"

export default function Home() {
  const [year, setYear] = useState<string>("")
  const [month, setMonth] = useState<string>("")
  const [day, setDay] = useState<string>("")
  const [chartSvg, setChartSvg] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  // Generate years from 1980 to current year
  const currentYear = new Date().getFullYear()
  const years = Array.from({ length: currentYear - 1989 }, (_, i) => (currentYear - i).toString())

  // Months for dropdown
  const months = [
    { value: "01", label: "January" },
    { value: "02", label: "February" },
    { value: "03", label: "March" },
    { value: "04", label: "April" },
    { value: "05", label: "May" },
    { value: "06", label: "June" },
    { value: "07", label: "July" },
    { value: "08", label: "August" },
    { value: "09", label: "September" },
    { value: "10", label: "October" },
    { value: "11", label: "November" },
    { value: "12", label: "December" },
  ]

  // Days for dropdown (will be filtered based on month/year)
  const getDaysInMonth = (year: string, month: string) => {
    if (!year || !month) return Array.from({ length: 31 }, (_, i) => (i + 1).toString().padStart(2, "0"))

    const daysInMonth = new Date(Number.parseInt(year), Number.parseInt(month), 0).getDate()
    return Array.from({ length: daysInMonth }, (_, i) => (i + 1).toString().padStart(2, "0"))
  }

  const days = getDaysInMonth(year, month)

  // Function to fetch chart SVG
  const fetchChartSvg = async (year?: string, month?: string, day?: string) => {
    if (!year || !month || !day) {
      // If no date is selected, fetch the latest chart
      fetchLatestChart()
      return
    }

    const dateStr = `${year}-${month}-${day}`
    const dateObj = parse(dateStr, "yyyy-MM-dd", new Date())

    if (!isValid(dateObj)) {
      setError("Invalid date selected")
      return
    }
    if (isWeekend(dateObj)) {
      setError("Treasury data is not available on weekends. Please select a weekday.")
      return
    }
    if (dateObj > new Date()) {
      setError("Future dates are not available. Please select a past date.")
      return
    }

    setIsLoading(true)
    setError(null)

    try {
      // In a real app, this would fetch the SVG from the backend
      const response = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/yield-chart?date=${dateStr}`)

      if (!response.ok) {
        setError(`Missing complete data for ${dateStr}.`)
        setIsLoading(false)
        return
      }

      const svgData = await response.text()
      setChartSvg(svgData)
      setIsLoading(false)
    } catch (error) {
      console.error("Error fetching chart SVG:", error)
      setError("Failed to load chart. Please try another date.")
      setIsLoading(false)
    }
  }

  // Function to fetch the latest chart
  const fetchLatestChart = async () => {
    setIsLoading(true)
    setError(null)

    try {
      // In a real app, this would fetch the latest SVG from the backend
      const response = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/yield-chart`)

      if (!response.ok) {
        throw new Error(`Error fetching chart: ${response.statusText}`)
      }

      const svgData = await response.text()
      setChartSvg(svgData)
      setIsLoading(false)
    } catch (error) {
      console.error("Error fetching latest chart SVG:", error)
      setError("Failed to load the latest chart.")
      setIsLoading(false)
    }
  }

  // Initial data fetch
  useEffect(() => {
    fetchLatestChart()
  }, []) // Removed fetchLatestChart as a dependency

  // Handle download
  const handleDownload = async (type: "csv" | "xlsx") => {
    let url = `${process.env.NEXT_PUBLIC_API_URL}/download?type=${type}`

    if (year && month && day) {
      const dateStr = `${year}-${month}-${day}`
      url += `&date=${dateStr}`
    }

    // In a real app, this would trigger a file download
    window.open(url, "_blank")
  }

  return (
    <div className="container mx-auto py-8 px-4">
      <h1 className="text-3xl font-bold mb-2">U.S. Treasury Yield Curve & Zero-Rate Analysis</h1>
      <p className="text-gray-500 mb-6">
        Select a historical date to view the U.S. Treasury par yield curve and bootstrapped zero-rate curve.
      </p>

      <div className="flex flex-col md:flex-row justify-between items-start md:items-center mb-6 gap-4">
        <div className="flex flex-wrap gap-2 w-full md:w-auto">
          <div className="w-full md:w-auto">
            <Select value={year} onValueChange={(value) => {
            setYear(value)
            if (value && month && day) {
            fetchChartSvg(value, month, day)
            }
              }}>
              <SelectTrigger className="w-full md:w-[120px]">
                <SelectValue placeholder="Year" />
              </SelectTrigger>
              <SelectContent>
                {years.map((y) => (
                  <SelectItem key={y} value={y}>
                    {y}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          <div className="w-full md:w-auto">
            <Select value={month} onValueChange={(value) => {
          setMonth(value)
          if (year && value && day) fetchChartSvg(year, value, day)
              }}>
              <SelectTrigger className="w-full md:w-[130px]">
                <SelectValue placeholder="Month" />
              </SelectTrigger>
              <SelectContent>
                {months.map((m) => (
                  <SelectItem key={m.value} value={m.value}>
                    {m.label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          <div className="w-full md:w-auto">
            <Select value={day} onValueChange={(value) => {
          setDay(value)
          if (year && month && value) fetchChartSvg(year, month, value)
              }}>
              <SelectTrigger className="w-full md:w-[100px]">
                <SelectValue placeholder="Day" />
              </SelectTrigger>
              <SelectContent>
                {days.map((d) => (
                  <SelectItem key={d} value={d}>
                    {d}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
        </div>

        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant="outline" className="w-full md:w-auto">
              <Download className="mr-2 h-4 w-4" />
              Download Data
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent>
            <DropdownMenuItem onClick={() => handleDownload("csv")}>Download CSV</DropdownMenuItem>
            <DropdownMenuItem onClick={() => handleDownload("xlsx")}>Download Excel</DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Yield Curve Analysis</CardTitle>
          <CardDescription>
            {year && month && day
              ? `Data for ${months.find((m) => m.value === month)?.label} ${day}, ${year}`
              : "Latest available data"}
          </CardDescription>
        </CardHeader>
        <CardContent>
          {isLoading ? (
            <div className="w-full flex items-center justify-center">
              <Skeleton className="w-full h-[350px] rounded-lg" />
            </div>
          ) : error ? (
            <div className="w-full flex items-center justify-center">
              <div className="text-center text-destructive">
                <p>{error}</p>
                <Button variant="outline" className="mt-4" onClick={fetchLatestChart}>
                  View Latest Data
                </Button>
              </div>
            </div>
          ) : (
            <div
              className="w-full flex items-center justify-center"
              dangerouslySetInnerHTML={{ __html: chartSvg || "" }}
            />
          )}
        </CardContent>
      </Card>

      <div className="mt-6 text-sm text-gray-500">
        <p>Source: U.S. Department of the Treasury</p>
      </div>
    </div>
  )
}

