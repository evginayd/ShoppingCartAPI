# 🛒 ShoppingCartAPI

An e-commerce API for managing shopping carts, product catalog, user accounts, and payment processes, built with ASP.NET Core.

## 🌟 Project Architecture & Design

This project adheres to **Three-Tier Architecture** and **Dependency Inversion Principle (DIP)** (part of SOLID) to ensure maintainability, scalability, and testability.

### Key Architectural Layers

| Folder | Layer | Responsibility | Abstraction Use |
| :--- | :--- | :--- | :--- |
| `Controllers` | Presentation / API | Handles HTTP requests, routes, and responses. Depends only on **Services** interfaces. | Loosely coupled from business logic. |
| `Services` | Business Logic | Contains core business rules (e.g., inventory check, price calculation). Depends only on **Repository** interfaces. | Fully testable with mocked repositories. |
| `Repositories` | Data Access | Manages database operations (CRUD operations). Implements the **Repository Interfaces**. | Isolates the service layer from the database technology (EF Core). |
| `Interfaces` | Abstraction | Defines the contracts for all Repositories and Services, enabling Dependency Injection. | Essential for loose coupling and Unit Testing. |
| `Models` | Domain | Contains entity classes (POCOs) representing the database structure. | Shared across all layers. |
| `Dto` | Data Transfer Objects | Defines objects used for data communication between the API and clients (request/response models). | Prevents over-posting/under-posting and separates domain models from exposure. |

## 🛠️ Technologies & Tools

* **Framework:** .NET 8 / ASP.NET Core
* **Database:** Entity Framework Core (Code-First Migrations)
* **Security:** JWT (JSON Web Tokens) Authentication
* **Language:** C#
* **Testing:** xUnit / NUnit (Recommended for Unit and Integration Testing)

## 🚀 Getting Started

Follow these instructions to set up and run the project locally.

### Prerequisites

* .NET SDK (Version 8.0 or later)
* A preferred IDE (Visual Studio, Rider, VS Code)

### 1. Database Setup

This project uses Entity Framework Core Migrations.

1.  **Update Connection String:** Open `appsettings.json` and configure the `DefaultConnection` string for your local database environment (e.g., SQL Server LocalDB).
2.  **Apply Migrations:** Use the .NET CLI from the project root directory:
    ```bash
    dotnet ef database update
    ```
3.  **Seed Data (Optional):** If `Seed.cs` contains initial data, run the application to execute the seeding logic (or manually run the seeding methods if applicable).

### 2. Run the Application

You can run the application using the following command:

```bash
dotnet run
