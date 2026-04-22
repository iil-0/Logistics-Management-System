# LogiTech - Logistics Management System

LogiTech is a comprehensive full-stack cargo management platform designed with a focus on software engineering best practices and advanced design patterns. The system provides a robust environment for managing shipments, tracking cargo in real-time, and calculating costs based on various parameters.

## Project Overview

The platform allows users to register, log in, and manage their shipments through a modern web interface. It calculates shipping costs by considering parcel types, additional services, and transportation methods. The backend is built using .NET 8, while the frontend utilizes React with Vite.

## Architecture and Design Patterns

The core strength of LogiTech lies in its implementation of six major software design patterns to ensure scalability, maintainability, and clean code.

### 1. Factory Pattern (Creational)
Used to instantiate different types of parcels based on user selection without exposing the creation logic to the client. The system supports Standard, Fragile, and Heavy Load parcel types, each with its own base cost and characteristics.

### 2. Decorator Pattern (Structural)
Allows for the dynamic addition of extra services to a parcel object at runtime. Services such as Insurance Coverage and Fast Delivery are implemented as decorators, wrapping the base parcel object to extend its price and description without modifying the original class.

### 3. Strategy Pattern (Behavioral)
Enables the calculation of transport costs using different algorithms based on the chosen transport method (Airway, Roadway, or Seaway). Each method is a separate strategy class, making it easy to add new transport logic in the future.

### 4. Observer Pattern (Behavioral)
Manages the notification system. When a shipment's status changes, the system notifies registered observers (such as internal notification services and email simulators) to keep the user informed about their cargo's progress.

### 5. State Pattern (Behavioral)
Manages the lifecycle of a shipment. A shipment transitions through various states: Order Received, Preparing, On the Way, Delivered, or Cancelled. Each state defines whether a shipment can be cancelled or advanced to the next stage.

### 6. Command Pattern (Behavioral)
Encapsulates all shipment operations (Create, Update Status, Cancel) as command objects. This approach allows for detailed operation logging and provides the infrastructure for "Undo" functionality, enabling actions to be reverted safely.

## Technical Stack

### Backend
- Framework: .NET 8 (ASP.NET Core Web API)
- Language: C#
- Authentication: Cookie-based Authentication
- Architecture: Controller-Service-Command Pattern

## Project Structure

LogiTech/
├── LogiTechAPI/              (Backend Source Code)
│   ├── Command/              (Command Pattern Implementation)
│   ├── Controllers/          (API Endpoints)
│   ├── Decorator/            (Structural Decorators)
│   ├── DTOs/                 (Data Transfer Objects)
│   ├── Factory/              (Creational Logic)
│   ├── Models/               (Domain Entities)
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

## Getting Started

### Prerequisites
- .NET 8 SDK
- Node.js (Version 18 or higher)

### Installation

1. Navigate to the LogiTech directory.
2. To start the backend:
   - Navigate to the LogiTechAPI folder.
   - Run the command: dotnet run
   - The API will be available at http://localhost:5085

3. To start the frontend:
   - Navigate to the lojistik-frontend folder.
   - Run the command: npm install (only for the first time)
   - Run the command: npm run dev
   - The application will be available at http://localhost:5173

## Functional Features

- User Authentication: Secure registration and login system.
- Shipment Creation: Dynamic cost calculation with real-time feedback.
- Real-time Tracking: Detailed status history for every shipment.
- My Shipments: Personalized dashboard for users to manage their active and past cargo.
- Responsive Design: Fully optimized for both desktop and mobile devices.
