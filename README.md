# LogiTech - Design Patterns Implementation Project

LogiTech is a full-stack cargo and logistics management system. It was developed primarily as an educational project to practice and demonstrate the use of **Object-Oriented Design Patterns** in a real-world scenario. 

While the project has a working React frontend for users to create and track shipments, the main focus is on the backend architecture. Instead of writing simple, long blocks of code, the backend was specifically structured to show how design patterns can solve common coding problems and keep the code clean.

## Project Overview

In this system, users can register, log in, and create cargo shipments. When a shipment is created, the total price is calculated dynamically based on the package type, the transport method, and any extra services chosen by the user.

The backend is built with **.NET 8** and uses **Entity Framework Core** to connect to a **PostgreSQL** database. The frontend is built using **React** and **Vite**.

## Design Patterns Used in the Project

To avoid "spaghetti code" and to make the system easier to understand, 6 different design patterns were implemented in the core logic:

### 1. Factory Pattern (Creational)
This pattern is used to create different types of packages. Instead of using complex `if-else` statements to check what the user selected, the Factory pattern automatically generates the correct object (`Standard`, `Fragile`, or `Heavy Load`). Each object has its own specific base price.

### 2. Decorator Pattern (Structural)
This pattern handles the extra services. If a user wants to add `Insurance` or `Fast Delivery` to their shipment, the Decorator pattern "wraps" the base package with these new features. This adds the extra costs to the total price without modifying the original package classes.

### 3. Strategy Pattern (Behavioral)
This pattern calculates the final shipping cost based on how the cargo will travel. The math for `Airway`, `Roadway`, and `Seaway` is separated into different strategy classes. This makes the code very organized and makes it easy to add a new transport method later.

### 4. Observer Pattern (Behavioral)
This pattern is used for the notification system. When a shipment's status changes (for example, from "Preparing" to "On the Way"), the system automatically updates the observers. This keeps the different parts of the system separated but still communicating.

### 5. State Pattern (Behavioral)
This pattern controls the lifecycle of a cargo. A shipment can be in different states like `Order Received`, `Preparing`, `On the Way`, `Delivered`, or `Cancelled`. The State pattern ensures that logical rules are followed (for example, a package cannot be cancelled if it is already on the way).

### 6. Command Pattern (Behavioral)
This pattern manages the main actions, like creating a shipment or updating its status. Every action is turned into a separate command object. This keeps the API controllers very clean and makes it possible to easily add an "Undo" feature to reverse actions.

## Technical Stack

### Backend
- **Framework:** .NET 8 (ASP.NET Core Web API)
- **Database:** PostgreSQL (Entity Framework Core)
- **Authentication:** Cookie-based Authentication

### Frontend
- **Framework:** React with Vite
- **Styling:** Vanilla CSS (Navy Blue Theme)

## Project Structure

The folders in the backend were specifically named after the design patterns to make the structure clear and easy to follow.

```text
LogiTech/
├── LogiTechAPI/              (Backend Source Code)
│   ├── Command/              (Command Pattern Implementations)
│   ├── Controllers/          (API Endpoints)
│   ├── Data/                 (EF Core DbContext & Migrations)
│   ├── Decorator/            (Structural Decorators)
│   ├── DTOs/                 (Data Transfer Objects - Requests & Responses)
│   ├── Factory/              (Creational Logic)
│   ├── Models/               (Database Entities)
│   ├── Observer/             (Notification System)
│   ├── Services/             (Business Logic Layer)
│   ├── State/                (State Pattern Logic)
│   └── Strategy/             (Calculation Strategies)
│
└── lojistik-frontend/        (Frontend Source Code)
    ├── src/
    │   ├── components/       (Reusable UI Components)
    │   ├── context/          (Authentication Context)
    │   ├── pages/            (Page-level Components)
    │   └── App.jsx           (Root Component and Routing)
```

## How to Run the Project

### Requirements
- .NET 8 SDK
- Node.js (v18+)
- PostgreSQL Database

### Setup Instructions

1. **Database Setup:**
   Open a terminal in the `LogiTechAPI` folder and run this command to create the database tables:
   ```bash
   dotnet ef database update
   ```

2. **Start the Backend API:**
   In the same `LogiTechAPI` folder, run:
   ```bash
   dotnet run
   ```
   *The API will be running at `http://localhost:5085`*

3. **Start the Frontend:**
   Open a new terminal, go to the `lojistik-frontend` folder, and run:
   ```bash
   npm install
   npm run dev
   ```
   *The website will be running at `http://localhost:5173`*

## API Endpoints

Here are the primary API routes used in the project:

| Endpoint | HTTP Method | Description |
| :--- | :--- | :--- |
| `/api/auth/register` | `POST` | Registers a new user. |
| `/api/auth/login` | `POST` | Authenticates a user and sets a cookie. |
| `/api/auth/me` | `GET` | Retrieves the currently logged-in user's details. |
| `/api/cargo/create-shipment` | `POST` | Calculates total price and creates a new shipment. |
| `/api/cargo/my-shipments` | `GET` | Retrieves all shipments belonging to the logged-in user. |
| `/api/cargo/track/{trackingNo}` | `GET` | Retrieves the current status and history of a specific shipment. |
| `/api/cargo/update-status/{trackingNo}`| `POST` | Advances the status of a shipment (requires auth). |
| `/api/cargo/cancel/{trackingNo}` | `POST` | Cancels a shipment if it is still in the "Order Received" state. |

## Screenshots

*(Add your project screenshots here. You can drag and drop images directly into GitHub's README editor, or save them in an `assets/` folder and link them below.)*

<!-- Example: ![LogiTech Dashboard](assets/dashboard.png) -->

