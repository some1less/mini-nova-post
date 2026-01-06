
<img width="1057" height="457" alt="Screenshot 2026-01-03 at 21 27 14" src="https://github.com/user-attachments/assets/37584c22-7f60-4926-a651-5a7984ef1ad6" />

# Mini Nova Post

This repository provides an overview of the **Mini Nova Post** ecosystem — a streamlined logistics simulation platform inspired by modern postal services.

**Mini Nova Post** is an application designed to bridge the gap between clients and logistics operations. It empowers users to manage digital documents for their parcels and interact with delivery services in a few clicks, eliminating traditional paperwork and queues. At the heart of this project are **efficiency**, **transparency**, and **reliability**.

# Technology Stack

The project utilizes a modern, robust stack to ensure high performance and maintainability:

* **Backend**: .NET 8 (C#) following Clean Architecture.
* **Frontend**: React (SPA) for a seamless user interface.
* **Database**: SQLite for local development; architected for PostgreSQL in production.
* **ORM**: Entity Framework Core with Repository/Service patterns.

# Features

The ecosystem is designed with a focus on two core user experiences:

* **Client Experience**: Access to a personal cabinet with digital parcel history, real-time tracking, and simplified shipment creation.
* **Operator Experience**: Advanced tools for shipment processing, secure data handling, and lifecycle management of every parcel in the network.

# How to install and built

This section describes the standard process for setting up the local development environment. Before proceeding, ensure that **.NET 8 SDK** and **Node.js (LTS)** are installed on your system.

### Build Process

1. **Clone the repository** to your local machine.
2. **Environment Configuration:** Create an `appsettings.json` file in the `src/MiniNova.API` directory. Use the template below to ensure the JWT Authentication and Database connection are properly configured:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "MNPConnection": "Data Source=nova.db"
  },
  "Jwt": {
    "Issuer": "http://localhost:server_port",
    "Audience": "http://localhost:client_port",
    "Key": "put_your_long_key_here(min 64 characters)",
    "ValidInMinutes": 60
  }
}
```
3. **Database Initialization:** The project follows a **Code-First** approach using **EF Core** and **SQLite**. **SQLite** is a lightweight, file-based database engine that requires no external server installation.
   
   * Run `dotnet ef database update` within the API project folder to apply migrations and generate the `nova.db` file.
   * **Sample Data**: While the application includes built-in **Data Seeding** in `Program.cs` for initial setup, a dedicated `scripts/` directory next to `src/` is provided. Inside, you will find `insert.sql`, which can be used to manually populate or reset the database with sample users, operators, and parcel records.

4. **Running the Application:**
   * **Backend**: Execute `dotnet run` inside the `src/MiniNova.API` folder.
   * **Frontend**: Navigate to `src/client`, run `npm install`, and then `npm run dev`.
