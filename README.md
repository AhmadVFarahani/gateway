# Gateway Admin API

This project is a modular, scalable, and cleanly architected API Gateway Admin Panel built with .NET 8, following Clean Architecture and Best Practices.  
It is designed to manage services, routes, users, and other core components for a future API Gateway system.

---

## 🚀 Technologies Used

- .NET 8 Web API
- Entity Framework Core (SQL Server)
- AutoMapper
- FluentValidation
- Swagger (Swashbuckle)
- Modular Dependency Injection
- Clean Architecture (Domain-Driven Design Style)

---

## 📚 Project Structure

```plaintext
GatewaySolution/
├── Gateway.Admin.Api/         # API Layer (Controllers, Program.cs)
├── Gateway.Application/       # Application Layer (DTOs, Services, Validators)
├── Gateway.Domain/            # Domain Layer (Entities, Interfaces)
├── Gateway.Infrastructure/    # Infrastructure Layer (Repositories)
├── Gateway.Persistence/       # Persistence Layer (DbContext, Migrations)
```

---

## ⚙️ How to Run Locally

1. Clone the repository:
   ```bash
   git clone https://github.com/AhmadVFarahani/gateway.git
   cd gateway
   ```

2. Update your `appsettings.json` connection string:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=GatewayDb;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```

3. Apply database migrations:
   ```bash
   dotnet ef database update --project Gateway.Persistence --startup-project Gateway.Admin.Api
   ```

4. Run the API:
   ```bash
   dotnet run --project Gateway.Admin.Api
   ```

5. Navigate to Swagger UI for API exploration:
   ```
   https://localhost:5001/swagger
   ```

---

## 📈 Main Features Implemented

- Create, Read, Update, Delete (CRUD) operations for Services.
- Clean Architecture with complete separation of concerns.
- AutoMapper configuration for mapping between Entities and DTOs.
- FluentValidation for request validation.
- Modular Dependency Injection.
- Swagger UI available for quick testing and documentation.

---

## ✨ Future Improvements (Planned)

- Role-based Authorization
- API Gateway Middleware for Request Forwarding
- Docker and Docker Compose Setup
- Health Check Endpoint
- Automated Route Importer via Swagger/OpenAPI files
- API Key Authentication and Rate Limiting
- Billing and Usage Tracking System

---

## 📬 Contact

Feel free to reach out regarding this project or any future collaboration opportunities!

**Ahmad V. Farahani**

- LinkedIn: [Ahmad V. Farahani](https://www.linkedin.com/in/ahmadvfarahani/)
- GitHub: [AhmadVFarahani](https://github.com/AhmadVFarahani)

