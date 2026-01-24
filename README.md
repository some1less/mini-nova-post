<img width="1939" height="627" alt="Frame 1-2" src="https://github.com/user-attachments/assets/ea6a365a-086f-4f0b-80c7-8df8fd32a77e" />

# Mini Nova Post

This repository gives an overview of the **Mini Nova Post** ecosystem, a simple logistics simulation platform inspired by the modern postal service **[Nova Post](https://novapost.com)**.

**Mini Nova Post** is a web application designed to master .NET and React knowledge. It allows users to send and manage their parcels and interact with delivery services in a few clicks, eliminating traditional paperwork and queues. At the heart of this project are **efficiency**, **transparency**, and **reliability**.

# Technology Stack

The project utilizes a modern, robust stack to ensure high performance and maintainability:

* **Backend**: .NET 8
* **Frontend**: React (SPA)
* **Database**: PostgreSQL
* **ORM**: Entity Framework Core
* **Containerization**: Frontend, Backend, and database are containerized separately using Docker

# Features

The ecosystem is designed with a focus on two core user experiences:

* **Client Experience**: Access to a personal cabinet with digital parcel history, real-time tracking, and simplified shipment creation.
* **Operator Experience**: Advanced tools for shipment processing, secure data handling, and lifecycle management of every parcel in the network.

# How to Install and Build

This section describes the process for setting up the local development environment using **Docker**. This ensures that the Database, API, and all dependencies are configured automatically.

### Prerequisites
* **Docker Desktop** installed and running.
* (Optional) .NET 8 SDK and Node.js if you plan to run services without Docker.

### Quick Start with Docker

1. **Clone the repository** to your local machine.

2. **Environment Configuration:**
   Create a `.env` file in the root directory of the project (next to `docker-compose.yaml`). Copy the following template and fill in your secure keys:

```properties
# Database Configuration
POSTGRES_DB=XXXX
POSTGRES_USER=XXXX
POSTGRES_PASSWORD=XXXX

# JWT Configuration
JWT_KEY=YourSuperSecretKeyMustBeAtLeast64CharactersLong12345
JWT_ISSUER=http://localhost:XXXX
JWT_AUDIENCE=http://localhost:XXXX
JWT_VALID_MINUTES=60
```

3. **Run the Application:** Open your terminal in the project root and execute:
```Bash

docker-compose up --build
```

4. Access the Application:

* **API / Swagger:** `http://localhost:5050/swagger`
* **Frontend:** `http://localhost:5173`

# Extras

For all other additional features you can refer to this paragraph.

## Database Schema

<img width="2106" height="1234" alt="image" src="https://github.com/user-attachments/assets/78dc2ca0-5287-4230-8001-f015bb7ef48a" />

