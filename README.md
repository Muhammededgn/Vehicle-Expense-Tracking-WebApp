# Vehicle-Expense-Tracking-WebApp

A local-ready **Vehicle Expense Tracking** web application built with **Blazor (.NET 8)** and **Microsoft SQL Server (MSSQL)**.  
Track multiple vehicles, log expenses (fuel, maintenance, etc.), and view reports including fuel consumption and vehicle comparisons.

---

## Key Features

- **Multiple Vehicles**: Add and manage as many vehicles as you want.
- **Expense Tracking**: Log unlimited expenses per vehicle (fuel, maintenance, repairs, insurance, etc.).
- **Expense Listing**: View and filter expense history per vehicle.
- **Fuel Consumption Report**: Using **fuel liters** and **odometer/km** data, view **L/100km** consumption statistics.
- **Yearly Insights**: Review yearly summaries and compare trends across years.
- **2-Vehicle Comparison**: Compare up to **two vehicles** side-by-side (costs, consumption, totals).
- **Local & Private**: Data is stored locally on your SQL Server instance.

---

## Tech Stack

- **Blazor Web App** (.NET 8)
- **C#**
- **Entity Framework Core (Code First + Migrations)**
- **Microsoft SQL Server (Local)**

---

## Prerequisites

- **.NET SDK 8.x**
- **SQL Server** (choose one)
  - SQL Server Express / Developer Edition
  - OR LocalDB (commonly installed with Visual Studio)
- (Recommended) **Visual Studio 2026**

---

## Getting Started (Local Setup)
---
### 1) Clone the repository

```bash
git clone https://github.com/Alperen-Tasdemir/Vehicle-Expense-Tracking-Web-App.git
cd Vehicle-Expense-Tracking-Web-App
```
### 2) Configure SQL Server connection string
Open appsettings.json (or appsettings.Development.json) and set your local SQL Server instance.
Example (SQL Server Express):
```bash
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOURSERVERNAME;Database=VehicleExpenseTrackingDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
```
### 3)Database Initialization (Migrations)
This project uses EF Core migrations. On first setup, creating tables is automatic once you run database update.

#### 1) Option A — Visual Studio (Package Manager Console)
Open Tools → NuGet Package Manager → Package Manager Console then Run

#### 2)Option B — .NET CLI
Install EF tool if needed:
```bash
dotnet tool install --global dotnet-ef
```
Then update the database:
```bash
dotnet ef database update
```
Run the Application
```bash
dotnet run
```
Then open the URL shown in the console output (typically something like):
```bash
https://localhost:####
http://localhost:####
```

---
## How Fuel Consumption is Calculated

Fuel consumption is shown as Liters per 100 km (L/100km):

L/100km = (FuelLiters / DrivenKm) × 100

The accuracy depends on consistent fuel and kilometer/odometer entries.

## Suggested Workflow Inside the App

1) Add a vehicle

2) Enter expenses (fuel, maintenance, etc.)

3) Review lists and totals

4) Check fuel consumption reports

5) Compare up to two vehicles for yearly/overall insights

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you’d like to change.
