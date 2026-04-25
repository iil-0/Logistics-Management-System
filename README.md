# LogiTech — Logistics Management System

LogiTech is a full-stack cargo and logistics management platform developed as an educational project for the Software Design Patterns course. The system enables users to register, create cargo shipments, and track them through their lifecycle, while the backend demonstrates the practical application of six classical Object-Oriented Design Patterns within a real-world domain.

## 1. Project Overview

The primary objective of this project is to illustrate how design patterns can be employed to produce maintainable, extensible, and well-structured backend code. While a functional React-based frontend is provided to enable end-user interaction, the core focus of the project lies in the backend architecture.

When a user creates a shipment, the total price is computed dynamically based on three factors: the package type, the chosen transport method, and any optional services selected by the user. Each of these concerns is handled by a distinct design pattern, ensuring a clean separation of responsibilities throughout the system.

## 2. Key Features

- Cookie-based user authentication (registration, login, logout)
- Shipment creation, tracking, and lifecycle management
- Dynamic price calculation based on package type, transport method, and extra services
- Status history maintained for each shipment
- Support for multiple transport methods (Air, Land, Sea)
- Optional extra services (Insurance, Fast Delivery)
- Modular API architecture organised around design patterns

## 3. Design Patterns

Six design patterns were implemented in the backend, each addressing a distinct architectural concern.

### 3.1 Factory Pattern (Creational)

Used for the creation of different package types. Rather than relying on conditional branching to instantiate objects, the Factory pattern produces the appropriate package object — `Standard`, `Fragile`, or `HeavyLoad` — based on the user's selection. Each package type encapsulates its own base price.

### 3.2 Decorator Pattern (Structural)

Manages optional services such as Insurance and Fast Delivery. The Decorator pattern wraps the base package object, dynamically attaching additional cost and behaviour without modifying the underlying package classes. This adheres to the Open/Closed Principle.

### 3.3 Strategy Pattern (Behavioural)

Encapsulates the cost calculation logic for each transport method. Airway, Roadway, and Seaway calculations are isolated in separate strategy classes, which allows new transport methods to be introduced without altering existing code.

### 3.4 Observer Pattern (Behavioural)

Underpins the notification subsystem. When the status of a shipment changes, all registered observers are notified automatically, decoupling the status-update logic from the components that react to it.

### 3.5 State Pattern (Behavioural)

Governs the lifecycle of a shipment. A shipment may be in one of the following states: `Order Received`, `Preparing`, `On the Way`, `Delivered`, or `Cancelled`. The State pattern enforces valid transitions between states (for example, a shipment that is already on the way cannot be cancelled).

### 3.6 Command Pattern (Behavioural)

Encapsulates user actions — such as creating a shipment or updating its status — as discrete command objects. This keeps the API controllers thin and provides a foundation for future extensions such as undo functionality or action logging.

## 4. Technical Stack

**Backend**
- Framework: .NET 8 (ASP.NET Core Web API)
- ORM: Entity Framework Core
- Database: PostgreSQL (Neon, remote instance)
- Authentication: Cookie-based authentication

**Frontend**
- Framework: React (with Vite)
- Styling: Vanilla CSS (Navy Blue theme)

**Auxiliary Tooling**
- DataMigrator: a .NET console application included in the repository for migrating data from a local PostgreSQL database to the remote Neon instance.

## 5. Project Structure

The backend folders are deliberately named after the design patterns they contain, in order to make the architectural intent of the project explicit.

```
LogiTech/
├── LogiTechAPI/                              (Backend source code)
│   ├── Command/                              (Command pattern implementations)
│   ├── Controllers/                          (API endpoints)
│   ├── Data/                                 (EF Core DbContext and migrations)
│   ├── Decorator/                            (Structural decorators for extra services)
│   ├── DTOs/                                 (Request and response models)
│   ├── Factory/                              (Factory pattern for package creation)
│   ├── Models/                               (Database entities)
│   ├── Observer/                             (Notification subsystem)
│   ├── Services/                             (Business logic layer)
│   ├── State/                                (State pattern for shipment lifecycle)
│   ├── Strategy/                             (Strategy pattern for transport methods)
│   ├── appsettings.json                      (Configuration with placeholder credentials)
│   └── appsettings.Development.json.example  (Example development configuration)
│
├── DataMigrator/                             (Database migration utility)
│   ├── Program.cs
│   └── DataMigrator.csproj
│
└── lojistik-frontend/                        (Frontend source code)
    ├── src/
    │   ├── components/                       (Reusable UI components)
    │   ├── context/                          (Authentication context)
    │   ├── pages/                            (Page-level components)
    │   └── App.jsx                           (Root component and routing)
    ├── package.json
    └── vite.config.js
```

## 6. Database Configuration

The project was originally developed against a local PostgreSQL instance and has since been migrated to a remote Neon PostgreSQL deployment for improved availability and ease of collaboration.

| Property         | Value                                                                       |
|------------------|-----------------------------------------------------------------------------|
| Local database   | PostgreSQL at `localhost:5432` (`LogiTechDB`)                               |
| Remote database  | Neon PostgreSQL (`neondb`, eu-central-1 region)                             |
| Migration tool   | `DataMigrator` console application (included in the repository)             |

### 6.1 Configuration Setup

A template configuration file is provided at `LogiTechAPI/appsettings.Development.json.example`. To configure the application locally, create a file named `appsettings.Development.json` in the same directory and populate it with the appropriate credentials:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=YOUR_HOST; Port=5432; Database=YOUR_DATABASE; Username=YOUR_USERNAME; Password=YOUR_PASSWORD; SslMode=Require; TrustServerCertificate=true"
  }
}
```

### 6.2 Data Migration Utility

The `DataMigrator` tool transfers data from a local PostgreSQL database to the remote Neon instance. Specifically, it:

1. Reads data from the local PostgreSQL database;
2. Migrates the `Users`, `Shipments`, and `StatusHistories` tables;
3. Clears existing remote data to avoid conflicts;
4. Updates auto-increment sequences to preserve identifier continuity;
5. Handles `DateTime` fields with appropriate timezone conversion.

To execute the migration:

```bash
cd DataMigrator
dotnet run
```

For production environments, connection strings should be supplied via environment variables or a dedicated secrets manager rather than hard-coded values.

## 7. Installation and Execution

### 7.1 Prerequisites

- .NET 8 SDK
- Node.js (version 18 or above)
- PostgreSQL (local or remote)

### 7.2 Steps

**Step 1 — Apply database migrations**

```bash
cd LogiTechAPI
dotnet ef database update
```

**Step 2 — Migrate existing data (optional)**

```bash
cd DataMigrator
dotnet run
```

**Step 3 — Run the backend API**

```bash
cd LogiTechAPI
dotnet run
```

The API will be available at `http://localhost:5085`.

**Step 4 — Run the frontend**

In a new terminal:

```bash
cd lojistik-frontend
npm install
npm run dev
```

The web application will be available at `http://localhost:5173`.

## 8. API Endpoints

| Endpoint                                  | Method | Description                                                                  |
|-------------------------------------------|--------|------------------------------------------------------------------------------|
| `/api/auth/register`                      | POST   | Registers a new user.                                                        |
| `/api/auth/login`                         | POST   | Authenticates a user and issues an authentication cookie.                    |
| `/api/auth/logout`                        | POST   | Logs out the currently authenticated user.                                   |
| `/api/auth/me`                            | GET    | Returns details of the currently authenticated user.                         |
| `/api/cargo/create-shipment`              | POST   | Computes the total price and creates a new shipment.                         |
| `/api/cargo/my-shipments`                 | GET    | Returns all shipments belonging to the authenticated user.                   |
| `/api/cargo/track/{trackingNo}`           | GET    | Returns the current status and full history of the specified shipment.       |
| `/api/cargo/update-status/{trackingNo}`   | POST   | Advances a shipment to the next status (requires authentication).            |
| `/api/cargo/cancel/{trackingNo}`          | POST   | Cancels a shipment, provided it is still in the `Order Received` state.      |

## 9. Security Considerations

The following points should be observed prior to any production deployment:

- Database credentials must not be committed to version control. Environment variables or a dedicated secrets manager should be used instead.
- The placeholder values in `appsettings.json` must be replaced with valid credentials in each deployment environment.
- The current implementation hashes passwords using SHA-256. For production use, this should be replaced with a password-hashing algorithm such as BCrypt or Argon2.
- Rate limiting should be applied to authentication endpoints in order to mitigate brute-force attacks.
- HTTPS should be enforced and cookies should be configured with the appropriate security flags (`Secure`, `HttpOnly`, `SameSite`).

## 10. Development Notes

- Example configuration files (suffixed with `.example`) are provided to assist new contributors in setting up their development environment.
- The `.gitignore` file is configured to exclude environment-specific configuration files from version control.
- Each design pattern is implemented in a dedicated module for the sake of clarity and pedagogical value.
- The `DataMigrator` tool is intended for development use; it should be secured or removed before production deployment.