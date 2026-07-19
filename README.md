# Hotel Manager Booking

Hotel Manager Booking is an ASP.NET Core 8 hotel operations platform for managing the complete guest journey—from room discovery and booking to payment, check-in, check-out, invoicing, and follow-up communication.

The project combines a customer-facing booking experience with operational workflows for reception staff and hotel administrators. Its modular web host also exposes integration endpoints for room, passenger, invoice, amenity, and statistics data.

## Demo

- [Main booking flow](https://drive.google.com/file/d/18C1R1XPtCD8siLFdgYA-yR-teWY61Xsy/view?usp=drive_link)
- [Administration flow](https://drive.google.com/file/d/1whk9kUXY8xnFzyDIKguAPObQbWY_cad4/view?usp=drive_link)
- [Reception workflow](https://drive.google.com/file/d/1e3OPdMkw01cLJmYTmwnViQrHSMcEpX_C/view?usp=drive_link)

## Product capabilities

- Browse rooms, view room details, check date-based availability, and create bookings.
- Register and authenticate users with cookie authentication, Google OAuth, and JWT-protected API access.
- Process booking payments through PayOS and VNPay.
- Support reception workflows for booking review, guest registration, check-in, check-out, room status, and booking calendars.
- Manage rooms, room types, amenities, passengers, invoices, hotel services, and statistics.
- Send email-related work through RabbitMQ producer/consumer components.
- Deliver real-time user notifications through SignalR.
- Run scheduled room-status refreshes, check-in reminders, check-out reminders, and late check-out calculations with Quartz.NET.
- Use Redis for optional distributed caching and locking, with an in-memory fallback for local development.
- Generate booking QR codes and maintain booking, payment, notification, review, and staff-action records.

## Architecture

```text
Browser / API clients
          |
          v
HotelManagement.Web
  ASP.NET Core MVC host
  Razor views and controllers
  Feature modules and API endpoints
  Authentication, payments, jobs, messaging, SignalR, Redis
          |
          v
HotelManagement.Infrastructure
  EF Core DbContext
  SQL Server models and mappings
  EF Core migrations
          |
          v
      SQL Server

External services: Google OAuth, PayOS, VNPay, RabbitMQ, Redis
```

`HotelManagement.Web` is the application host. Its `Modules` directory contains feature-oriented areas such as authentication, rooms, payments, invoices, amenities, passengers, statistics, notifications, RabbitMQ, Redis, QR-code generation, and scheduled jobs. `HotelManagement.Infrastructure` contains the scaffolded EF Core model layer, the `ManagermentHotelContext` database context, configuration helpers, and migrations.

## Repository structure

```text
hotel-manager-booking/
├── README.md
├── .gitignore
└── hotel-management-platform/
    ├── HotelManagement.sln
    ├── .env.example
    ├── HotelManagement.Web/
    │   ├── Home/
    │   ├── Modules/
    │   │   ├── AdminMPassengers/
    │   │   ├── AmentityModules/
    │   │   ├── Api/
    │   │   ├── AuthenSerive/
    │   │   ├── Invoices/
    │   │   ├── Payment/
    │   │   ├── RabbitMQConsumer/
    │   │   ├── RabbitMQProducer/
    │   │   ├── RedisServices/
    │   │   ├── Rooms/
    │   │   ├── Secheduler/
    │   │   ├── SignalRModels/
    │   │   └── Statistics/
    │   ├── ViewModel/
    │   ├── Views/
    │   ├── wwwroot/
    │   ├── Program.cs
    │   └── appsettings.json
    ├── HotelManagement.Infrastructure/
    │   ├── Configuration/
    │   ├── Migrations/
    │   └── Models/
    │       └── ManagermentHotelContext.cs
    └── HotelManagement.Tests/
        ├── InvoiceApiTest.cs
        └── TestValidationAuthen.cs
```

### Project responsibilities

| Project | Responsibility |
| --- | --- |
| `HotelManagement.Web` | Web application, Razor UI, MVC controllers, API modules, authentication, payments, background jobs, messaging, caching, and real-time notifications |
| `HotelManagement.Infrastructure` | EF Core context, SQL Server persistence models, model configuration, and database migrations |
| `HotelManagement.Tests` | xUnit tests for authentication validation and invoice API behavior |

## Domain and persistence model

The EF Core context currently maps the main hotel operations domain, including:

- Users, roles, tokens, notifications, and staff actions
- Rooms, room types, images, amenities, and room-amenity relationships
- Bookings, booking details, guests, payments, temporary PayOS bookings, and booking services
- Orders, invoices, hotel services, reviews, and supporting records

Database schema changes are stored in `HotelManagement.Infrastructure/Migrations`. The context source is `HotelManagement.Infrastructure/Models/ManagermentHotelContext.cs`.

## Technology stack

- .NET 8 and ASP.NET Core MVC
- Entity Framework Core 8 with SQL Server
- Razor Views, Bootstrap, jQuery, and JavaScript
- Cookie authentication, Google OAuth, and JWT bearer authentication
- Quartz.NET scheduled jobs
- SignalR real-time notifications
- RabbitMQ.Client asynchronous messaging
- StackExchange.Redis distributed caching and locking
- PayOS and VNPay payment integrations
- QRCoder QR-code generation
- xUnit and Moq testing

## Prerequisites

- .NET 8 SDK
- SQL Server
- Visual Studio 2022 or another .NET-compatible IDE
- RabbitMQ for email messaging features
- Redis when Redis caching or distributed locking is enabled
- Provider credentials for Google OAuth, PayOS, VNPay, and SMTP email when those features are used

## Local configuration

Configuration is read from `appsettings.json`, environment variables, and an optional `.env` file. `DotEnvLoader` searches upward from the current directory and the application base directory, so a local `.env` can be placed at the solution/repository level.

Create a local environment file from the supplied template:

```powershell
Copy-Item ".\hotel-management-platform\.env.example" ".\hotel-management-platform\.env"
```

At minimum, configure the SQL Server connection and JWT settings. Add provider credentials only for the integrations you intend to run:

```text
ConnectionStrings__SQL=Server=localhost;Database=HotelManagement;Trusted_Connection=True;TrustServerCertificate=True
Jwt__Key=replace-with-a-long-development-secret
Jwt__Issuer=https://localhost:7236
Jwt__Audience=https://localhost:7045
Redis__Enabled=false
RabbitMQ__Host=localhost
RabbitMQ__UserName=guest
RabbitMQ__Password=guest
```

The full list of supported keys is documented in `hotel-management-platform/.env.example`, including Google, Facebook, PayOS, VNPay, RabbitMQ, Redis, and SMTP settings. Never commit passwords, signing keys, payment credentials, OAuth secrets, or production connection strings.

## Getting started

Run the commands from the repository root.

### Restore and build

```powershell
dotnet restore ".\hotel-management-platform\HotelManagement.sln"
dotnet build ".\hotel-management-platform\HotelManagement.sln"
```

### Apply database migrations

The migrations are owned by `HotelManagement.Infrastructure`, while `HotelManagement.Web` supplies the startup configuration:

```powershell
dotnet ef database update `
  --project ".\hotel-management-platform\HotelManagement.Infrastructure\HotelManagement.Infrastructure.csproj" `
  --startup-project ".\hotel-management-platform\HotelManagement.Web\HotelManagement.Web.csproj"
```

Review the configured connection string before applying migrations to a shared or production database.

### Run the web application

```powershell
dotnet run --project ".\hotel-management-platform\HotelManagement.Web\HotelManagement.Web.csproj"
```

The development launch profiles use:

- `https://localhost:7045`
- `http://localhost:5299`

The default route opens the home introduction page. MVC and API routes are implemented by controllers under `HotelManagement.Web/Modules`.

## Testing

Run the test project:

```powershell
dotnet test ".\hotel-management-platform\HotelManagement.Tests\HotelManagement.Tests.csproj"
```

The current suite includes authentication password-validation tests and an invoice API controller test using Moq.

## Project status

This project is under active development. Some namespaces and folder names preserve historical conventions, including `Management_Hotel_2025`, `Mydata`, `Amentity`, `AuthenSerive`, and `Secheduler`. These names are part of the current source structure and should be considered when adding new modules or refactoring existing ones.

## Credits

Developed by [Pham Trung Duc](mailto:ptrungduc1011@gmail.com).

No license file is currently included. Add an appropriate license before distributing the project publicly.
