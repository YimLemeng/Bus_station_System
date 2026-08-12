# 🚌 Bus Station Management System (ប្រព័ន្ធគ្រប់គ្រងស្ថានីយរថយន្តក្រុង)

![C#](https://img.shields.io/badge/Language-C%23-blue?style=for-the-badge&logo=c-sharp)
![Framework](https://img.shields.io/badge/Framework-.NET%204.7.2-purple?style=for-the-badge&logo=.net)
![Database](https://img.shields.io/badge/Database-SQL%20Server-red?style=for-the-badge&logo=microsoft-sql-server)
![Architecture](https://img.shields.io/badge/Architecture-3--Tier%20%28UI--BLL--DAL%29-brightgreen?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-orange?style=for-the-badge)

A comprehensive, modern desktop application built using **C# Windows Forms**, **SQL Server**, and **RDLC Reporting Services** designed to manage bus station operations, online/offline ticket bookings, payment processing, schedule dispatching, driver & fleet allocations, and financial reporting.

---

## 🌟 Key Features (លក្ខណៈពិសេសរបស់ប្រព័ន្ធ)

- 🚌 **Bus Fleet Management**: Track buses, seating capacity, bus types, and operational status.
- 🗺️ **Routes & Scheduling**: Manage origin/destination routes, departure/arrival times, and bus assignments.
- 🎫 **Ticket Booking & Auto Seat Reservation**: Real-time booking management with automatic ticket generation upon payment confirmation.
- 💳 **Multi-Payment Gateway Integration**: Supports **Cash**, **QR Payment (KHQR)**, **Credit Card**, and **Bank Transfer** with duplicate payment prevention.
- 📊 **RDLC Financial & Operational Reports**:
  - **Revenue & Financial Report**: Daily, monthly, and custom date range financial metrics (`RevenueReport.rdlc`).
  - **Passenger Ticket Receipt**: Printable receipts with passenger details, seat number, and price (`TicketReceipt.rdlc`).
  - **Passenger Trip Manifest**: Passenger departure manifests categorized by bus trip (`PassengerManifest.rdlc`).
- 👥 **Customer & Staff Management**: Track passenger profiles, employee roles, and driver schedules.
- 🔒 **User Authentication & Role-Based Access Control**: Secure login system with session tracking.

---

## 🏗️ Architecture & Project Structure (រចនាសម្ព័ន្ធកូដ 3-Tier)

This project strictly follows the **3-Tier Architecture (UI -> BLL -> DAL -> Database)** for clean separation of concerns and high maintainability:
BusStationManagement/ ├── 📁 Forms/ # Presentation Layer (WinForms UI) │ ├── Dashboard.cs # Main KPI dashboard │ ├── BookingFrm.cs # Booking reservation UI │ ├── PaymentFrm.cs # Payment processing & receipt trigger │ ├── TicketFrm.cs # Ticket issuance UI │ ├── ReportFrm.cs # RDLC Report Viewer UI │ └── ... ├── 📁 BLL/ # Business Logic Layer │ ├── BookingBLL.cs # Validation & business rules for Bookings │ ├── PaymentBLL.cs # Payment method validation & auto-confirm logic │ ├── TicketBLL.cs # Ticket issuance business logic │ └── ... ├── 📁 DAL/ # Data Access Layer (ADO.NET + Stored Procedures) │ ├── DBConnection.cs # SQL Connection Manager │ ├── BookingDAL.cs # SQL execution for Bookings │ ├── PaymentDAL.cs # SQL execution for Payments & Views │ └── ... ├── 📁 Entity/ # Data Models & DTOs │ ├── Booking.cs │ ├── Payment.cs │ ├── Ticket.cs │ └── ... ├── 📁 Reports/ # RDLC Report Definitions │ ├── RevenueReport.rdlc # Financial Revenue XML definition │ ├── TicketReceipt.rdlc # Ticket Printable Receipt XML definition │ └── PassengerManifest.rdlc # Trip Manifest XML definition └── 📄 App.config # System & DB Connection String Configurations


---

## ⚙️ Prerequisites & Setup 

### 1. Database Configuration
Before running the application, make sure to attach or configure your local SQL Server instance. Update the database properties inside the `App.config` file to match your SQL Server environment credentials:

```xml
<connectionStrings>
    <add name="MyDbConnection"
         connectionString="Data Source=localhost\SQLEXPRESS;Initial Catalog=BusStationDB;Integrated Security=True;"
         providerName="System.Data.SqlClient" />
</connectionStrings>
