# MeterReadingApp

This project allows an Energy Company Account Manager to upload CSV files of customer meter readings and monitor their energy consumption.

---

## 🛠 Tech Stack

- Backend: .NET 8 Web API (C#)
- Database: SQL Server
- CSV Parsing: CsvHelper (C#)

---

## 🚀 Features

- Upload CSVs via `/meter-reading-uploads` endpoint
- Validate meter readings:
  - Must match a valid AccountId
  - Format: 5-digit MeterReadValue (e.g., 00321)
  - No duplicate readings
  - No older readings than the latest
- Return count of successes and failures

---

## 📦 Running Locally

### Backend (.NET)

```bash
cd MeterReadingAPI
dotnet restore
dotnet ef database update 
dotnet run
