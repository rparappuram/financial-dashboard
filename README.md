# **Yield Curve Analysis Web Application**

## **📌 Overview**

This project is a small technical assessment application designed to:

- Retrieve publicly available **US Treasury yield curve data**.
- Plot a **discrete-tenor par yield curve**.
- Compute and visualize a **continuous monthly zero-rate curve for 30 years forward**.
- Allow users to select **historical dates** to generate plots.
- Provide an option to **download the yield data** in CSV or Excel format.

This application consists of two parts:

- **Backend:** A `.NET` API that fetches and processes US Treasury yield curve data.
- **Frontend:** A `Next.js` client that displays the graphs and enables data downloads.

## **🛠️ Tech Stack**

- **Backend:** `.NET 9`, `C#`, `ASP.NET Core`
- **Frontend:** `Next.js`, `React`, `TypeScript`, `ShadCN UI`
- **Data Sources:** US Treasury API (XML data parsing)

---

## **🚀 Getting Started**

### **1️⃣ Clone the Repository**

```sh
# Clone the repository
$ git clone https://github.com/financial-dashboard.git

# Navigate into the project directory
$ cd financial-dashboard
```

### **2️⃣ Backend Setup (YieldCurveAPI)**

```sh
# Navigate to the .NET API directory
$ cd YieldCurveAPI

# Restore dependencies
$ dotnet build

# Run the API
$ dotnet run
```

By default, the API will be available at: **`http://localhost:5000`**

### **3️⃣ Frontend Setup (Next.js Client)**

```sh
# Navigate to the client directory
$ cd client

# Install dependencies
$ npm install

# Run the Next.js development server
$ npm run dev
```

By default, the frontend will be available at: **`http://localhost:3000`**

---

## **📌 Features**

✅ **Par Yield & Zero Rate Curve:** Visualizes the discrete par yield and bootstrapped zero rate curves.  
✅ **Historical Data Selection:** Allows users to choose historical dates and generate yield curve graphs.  
✅ **Data Export:** Provides options to download CSV or Excel files containing all retrieved and computed yield data.  
✅ **Backend Processing:** Implements XML data retrieval, bootstrapping for zero rates, and continuous forward rate calculation.  
✅ **Next.js UI:** A clean and responsive frontend for selecting dates and viewing yield curve graphs.

---

## **📂 Project Structure**

```
📦 financial-dashboard
 ┣ 📂 YieldCurveAPI  # .NET 6+ API Backend
 ┃ ┣ 📂 Controllers  # API endpoints (Yield, Download)
 ┃ ┣ 📂 Services     # Business logic (XML Parsing, Bootstrapping, Charting)
 ┃ ┣ 📂 Models       # Data structures
 ┃ ┗ 📜 Program.cs   # Entry point
 ┣ 📂 client         # Next.js Frontend
 ┃ ┣ 📂 components   # UI components
 ┃ ┣ 📂 pages        # Next.js pages
 ┃ ┗ 📜 package.json # Frontend dependencies
 ┣ 📜 README.md      # Project documentation
 ┗ 📜 .gitignore     # Git ignore rules
```

---

## **📥 API Endpoints**

### **1️⃣ Retrieve Yield Chart**

**GET** `/api/yield-chart?date=YYYY-MM-DD`

- Returns an **SVG** of the yield curve graph for the specified date.

### **2️⃣ Download Data**

**GET** `/api/download?date=YYYY-MM-DD&type=csv|xlsx`

- Returns **CSV/Excel** file with yield data and charts for the requested date.

---

---

## **📜 License**

This project is for **technical assessment purposes only** and is not intended for production use.

---
