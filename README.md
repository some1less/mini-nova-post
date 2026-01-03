
<img width="1057" height="457" alt="Screenshot 2026-01-03 at 21 27 14" src="https://github.com/user-attachments/assets/37584c22-7f60-4926-a651-5a7984ef1ad6" />

# Welcome

Mini Nova Post is a simulation project inspired by major logistics providers. It demonstrates a simplified workflow for parcel management and tracking.

The project was created so that users could simply create shipments, track parcel status, and log into the system securely. In addition, employees also play a vital role, having the ability to update parcel status and generally monitor the security of shipped parcels in digital format. 

# Technology Stack

* **Backend:** .NET 8 (C#)
* **Frontend:** React (JavaScript/TypeScript)
* **Database:** SQLite (Dev) / PostgreSQL (Prod)
* **ORM:** Entity Framework Core

# Features

The system is divided into two main roles:

### For Clients:
* **Create Shipments:** Easy creation of new parcels with size/weight calculation.
* **Tracking:** Real-time status updates for sent and received parcels.
* **Personal Cabinet:** View history of sent and received packages.

### For Operators (Employees):
* **Parcel Processing:** Update statuses (e.g., "In Transit", "Arrived").
* **Management:** Secure handling of shipment data.

# Architecture

The solution follows **Clean N-Layer Architecture** principles to ensure separation of concerns and scalability:

* **MiniNova.API**: Presentation layer (REST Controllers).
* **MiniNova.BLL**: Business Logic Layer (Services, DTOs, Interfaces).
* **MiniNova.DAL**: Data Access Layer (EF Core Context, Entities, Migrations).
* **Client**: Frontend application (React).
