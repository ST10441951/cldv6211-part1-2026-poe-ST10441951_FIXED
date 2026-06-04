Note, previous REPO for POE locked me ou, so I had to create a FIXED REPO under the same name. 
---
# EventEase | Cloud-Based Event & Venue Management System

**Project Code:** CLDV6211-Part-3-2026-POE  
**Student ID:** ST10441951  
**Author:** Joshua Marc Lourens  
**Academic Institution:** Westville - School of Computer Science

---

# 🌟 Project Overview

**EventEase** is a cloud-hosted event and venue management platform designed to streamline venue administration, event scheduling, and booking management within an organizational environment.

The application evolved through three development phases:

- **Part 1:** Database design and CRUD functionality using SQL Server and Entity Framework Core.
- **Part 2:** Integration of Azure Blob Storage and advanced booking validation logic.
- **Part 3:** Full cloud migration to Microsoft Azure, including Azure App Service, Azure SQL Database, and Azure Blob Storage.

This architecture provides improved scalability, security, maintainability, and accessibility while demonstrating modern cloud development practices.

---

# 🚀 Evolution of EventEase

## Part 1: Database Foundation

The first phase focused on building a robust relational data model using Entity Framework Core and SQL Server.

### Features Implemented

- Normalized database design
- Venue Management (CRUD)
- Event Management (CRUD)
- Booking Management (CRUD)
- Entity Relationships
- Navigation Properties
- Entity Framework Core Integration

### Database Structure

The application consists of three primary entities:

- **Venues**
- **Events**
- **Bookings**

Relationships between these entities ensure data consistency and prevent orphaned records.

---

## Part 2: Cloud Storage & Business Logic

The second phase introduced cloud-based file storage and improved booking management.

### Azure Blob Storage Integration

Venue images were migrated from local file storage to Azure Blob Storage using:

- Azurite Storage Emulator
- Azure Storage Explorer
- Azure Storage SDK for .NET

### Advanced Booking Validation

The booking system prevents overlapping reservations through conflict detection logic:

```csharp
(StartA <= EndB) && (EndA >= StartB)
```

### Referential Integrity Protection

Additional validation was implemented to:

- Prevent deletion of venues containing active events
- Prevent deletion of events linked to bookings
- Protect database consistency

### Administrative Search Features

A centralized booking ledger allows administrators to:

- Search bookings by Booking ID
- Search bookings by Event Name
- View related Venue and Event information

---

## Part 3: Azure Cloud Migration

The final phase focused on transforming EventEase into a production-ready cloud application.

### Azure Services Used

#### Azure App Service

The ASP.NET Core MVC application is hosted using Azure App Service.

Benefits include:

- Managed web hosting
- Simplified deployment
- Built-in scalability
- Secure environment management
- High availability

#### Azure SQL Database

All relational application data is stored in Azure SQL Database.

Benefits include:

- Managed database administration
- Automated backups
- High availability
- Secure cloud-based storage
- Entity Framework Core compatibility

#### Azure Blob Storage

Venue images are stored in Azure Blob Storage.

Benefits include:

- Scalable file storage
- Fast content delivery
- Reduced server workload
- Separation of structured and unstructured data

---

# 🏗 System Architecture

```text
User Browser
      │
      ▼
Azure App Service
(ASP.NET Core MVC)
      │
      ├──────────────► Azure SQL Database
      │                    │
      │                    ▼
      │              Venue/Event/Booking Data
      │
      └──────────────► Azure Blob Storage
                           │
                           ▼
                     Venue Images
```

This architecture follows modern cloud design principles by separating:

- Application Hosting
- Relational Data Storage
- File Storage

---

# ✨ Core Features

## Venue Management

- Create venues
- View venue details
- Edit venue information
- Delete venues
- Upload venue images

## Event Management

- Create events
- Manage event details
- Link events to venues
- Prevent orphaned event records

## Booking Management

- Create bookings
- Modify bookings
- Cancel bookings
- Prevent double-bookings
- Validate booking schedules

## Cloud Storage Integration

- Upload venue images
- Store files in Azure Blob Storage
- Retrieve images dynamically

## Search & Reporting

- Search bookings
- Filter records
- View related entity data
- Administrative booking ledger

---

# 🔒 Security & Configuration

To protect sensitive credentials, production settings are stored using Azure App Service Configuration.

The following values are secured:

- Azure SQL Connection String
- Azure Blob Storage Connection String
- Storage Account Credentials

This prevents confidential information from being exposed within source code repositories.

---

# 🛠 Technology Stack

| Component | Technology |
|------------|------------|
| Framework | ASP.NET Core MVC |
| Language | C# |
| ORM | Entity Framework Core |
| Database | Azure SQL Database |
| Cloud Hosting | Azure App Service |
| File Storage | Azure Blob Storage |
| Frontend | Bootstrap 5 |
| Styling | CSS3 |
| Markup | HTML5 |
| IDE | Visual Studio 2022 |
| Source Control | GitHub |
| Development Storage | Azurite Emulator |
| Storage Management | Azure Storage Explorer |

---

# ⚙ Installation & Local Development

## Prerequisites

- .NET SDK
- Visual Studio 2022
- SQL Server Express / LocalDB
- Azurite Storage Emulator
- Azure Storage Explorer

## Clone Repository

```bash
git clone https://github.com/YourUsername/EventEase.git
```

## Configure Database

Update the LocalDB connection string inside:

```json
appsettings.json
```

Apply Entity Framework migrations:

```powershell
Update-Database
```

## Start Azurite

Launch the Azurite emulator and verify that Blob Storage is running.

Default endpoint:

```text
http://127.0.0.1:10000/devstoreaccount1
```

## Run Application

Open the solution in Visual Studio 2022 and press:

```text
F5
```

Or run:

```bash
dotnet run
```

---

# ☁ Azure Deployment

The production deployment uses:

- Azure App Service
- Azure SQL Database
- Azure Storage Account (Blob Storage)
- Azure App Service Application Settings

### Environment Variables

Production secrets are stored securely within Azure App Service Configuration rather than inside source-controlled files.

Examples:

- `ConnectionStrings__EventEaseContext`
- `ConnectionStrings__AzureBlobStorage`

This follows industry best practices for secure cloud deployments.

---

# 📖 Lessons Learned

This project provided practical experience in:

- Cloud application deployment
- Azure App Service configuration
- Azure SQL Database management
- Azure Blob Storage integration
- Environment separation
- Secure credential management
- Cloud troubleshooting and diagnostics
- Enterprise application architecture

The migration from LocalDB and Azurite to Azure SQL Database and Azure Blob Storage demonstrated the importance of scalability, security, and proper cloud resource management in modern software development.

---

# 📚 References

- Microsoft Learn – ASP.NET Core Documentation
- Microsoft Learn – Azure App Service Documentation
- Microsoft Learn – Azure SQL Database Documentation
- Microsoft Learn – Azure Blob Storage Documentation
- Microsoft Learn – Entity Framework Core Documentation
- Bootstrap 5 Documentation

---

## 👨‍💻 Author

**Joshua Marc Lourens**  
**Student Number:** ST10441951

© 2026 Joshua Marc Lourens. All Rights Reserved.
