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
- Real email notifications dispatched via Gmail SMTP on each status change
- Modular API architecture organised around design patterns
- Containerised deployment via Docker and Docker Compose

## 3. Design Patterns

Six design patterns were implemented in the backend, each addressing a distinct architectural concern.

### 3.1 Factory Pattern (Creational)

Used for the creation of different package types. Rather than relying on conditional branching to instantiate objects, the Factory pattern produces the appropriate package object — `Standard`, `Fragile`, or `HeavyLoad` — based on the user's selection. Each package type encapsulates its own base price.

### 3.2 Decorator Pattern (Structural)

Manages optional services such as Insurance and Fast Delivery. The Decorator pattern wraps the base package object, dynamically attaching additional cost and behaviour without modifying the underlying package classes. This adheres to the Open/Closed Principle.

### 3.3 Strategy Pattern (Behavioural)

Encapsulates the cost calculation logic for each transport method. Airway, Roadway, and Seaway calculations are isolated in separate strategy classes, which allows new transport methods to be introduced without altering existing code.

### 3.4 Observer Pattern (Behavioural)

Underpins the notification subsystem. When the status of a shipment changes, all registered observers — including a `NotificationObserver` for in-memory logging and an `EmailObserver` that dispatches real emails through Gmail SMTP — are notified automatically. To ensure that observer subscriptions persist across HTTP requests, the registry holding them is implemented as a singleton (`ObserverRegistry`), decoupling subscription lifetime from the per-request lifecycle of the service layer.

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
- Email delivery: System.Net.Mail over Gmail SMTP

**Frontend**
- Framework: React (with Vite)
- Styling: Vanilla CSS (Navy Blue theme)

**Infrastructure**
- Docker and Docker Compose for containerised deployment of the API and frontend
- Nginx serves the production build of the frontend inside its container

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
│   ├── Observer/                             (Notification subsystem, including EmailObserver)
│   ├── Services/                             (Business logic layer, includes the singleton ObserverRegistry)
│   ├── Settings/                             (Strongly-typed configuration models, e.g. EmailSettings)
│   ├── State/                                (State pattern for shipment lifecycle)
│   ├── Strategy/                             (Strategy pattern for transport methods)
│   ├── Dockerfile                            (Container build definition for the API)
│   └── appsettings.json                      (Local configuration; ignored by git)
│
├── lojistik-frontend/                        (Frontend source code)
│   ├── src/
│   │   ├── components/                       (Reusable UI components)
│   │   ├── context/                          (Authentication context)
│   │   ├── pages/                            (Page-level components)
│   │   └── App.jsx                           (Root component and routing)
│   ├── Dockerfile                            (Container build definition for the frontend)
│   ├── package.json
│   └── vite.config.js
│
├── docker-compose.yml                        (Service composition for API and frontend)
└── .env                                      (Environment variables for Docker; ignored by git)
```

## 6. Configuration

### 6.1 Database

The project uses a remote Neon PostgreSQL deployment for improved availability and ease of collaboration. The connection string is supplied to the API in one of two ways:

- During local development (without Docker): via `LogiTechAPI/appsettings.json`
- When running through Docker Compose: via the `DB_CONNECTION_STRING` environment variable, typically declared in a local `.env` file

Both `appsettings.json` and `.env` are excluded from version control to prevent credentials from being committed.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=YOUR_HOST; Port=5432; Database=YOUR_DATABASE; Username=YOUR_USERNAME; Password=YOUR_PASSWORD; SslMode=Require; Channel Binding=Require"
  }
}
```

### 6.2 Email Notifications

The `EmailObserver` dispatches notifications to the shipment owner whenever the shipment status changes. Configuration is provided through the `EmailSettings` section of `appsettings.json`:

```json
{
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your-account@gmail.com",
    "AppPassword": "your-16-character-gmail-app-password"
  }
}
```

A Gmail account with two-factor authentication enabled is required, along with an App Password generated specifically for this application.

## 7. Installation and Execution

### 7.1 Prerequisites

- .NET 8 SDK (only required when running outside Docker)
- Node.js version 18 or above (only required when running the frontend outside Docker)
- Docker Desktop (required for the containerised setup)

### 7.2 Running with Docker (recommended)

The simplest way to run the entire stack is via Docker Compose. Ensure that a `.env` file containing the `DB_CONNECTION_STRING` variable is present at the repository root.

```bash
docker-compose up --build
```

Once the build completes, the services are available at:

- API: `http://localhost:5085`
- Frontend: `http://localhost:5173`

### 7.3 Running Locally Without Docker

**Step 1 — Apply database migrations**

```bash
cd LogiTechAPI
dotnet ef database update
```

**Step 2 — Run the backend API**

```bash
cd LogiTechAPI
dotnet run
```

The API will be available at `http://localhost:5085`.

**Step 3 — Run the frontend**

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
| `/api/cargo/all-shipments`                | GET    | Returns all shipments in the system (administrators only).                   |
| `/api/cargo/track/{trackingNo}`           | GET    | Returns the current status and full history of the specified shipment.      |
| `/api/cargo/update-status/{trackingNo}`   | POST   | Advances a shipment to the next status (administrators only).               |
| `/api/cargo/cancel/{trackingNo}`          | POST   | Cancels a shipment, provided it is still in the `Order Received` state.      |
| `/api/cargo/health`                       | GET    | Returns the current health status of the API.                                |

## 9. Security Considerations

The following points should be observed prior to any production deployment:

- Database credentials and SMTP App Passwords must not be committed to version control. Environment variables or a dedicated secrets manager should be used instead.
- The current implementation hashes passwords using SHA-256. For production use, this should be replaced with a password-hashing algorithm such as BCrypt or Argon2.
- Rate limiting should be applied to authentication endpoints in order to mitigate brute-force attacks.
- HTTPS should be enforced and cookies should be configured with the appropriate security flags (`Secure`, `HttpOnly`, `SameSite`).
- For email delivery in production, a dedicated transactional email provider (such as SendGrid or Mailgun) is recommended in place of personal Gmail SMTP credentials.

## 10. Development Notes

- Configuration files containing secrets (`appsettings.json`, `.env`) are excluded from version control via `.gitignore`.
- Each design pattern is implemented in a dedicated module for the sake of clarity and pedagogical value.
- The `ObserverRegistry` is registered as a singleton in the dependency-injection container, while domain services such as `GonderiService` remain scoped to each HTTP request; this combination allows observer subscriptions to outlive individual requests without compromising request-level isolation of the database context.
