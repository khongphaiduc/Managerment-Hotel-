# Hotel Manager Booking

An ASP.NET Core 8 hotel management platform for online room booking and day-to-day hotel operations. The application brings guest booking, room availability, front-desk workflows, payments, invoicing, administration, notifications, and reporting into a single web solution.

## Demo

- [Main booking flow](https://drive.google.com/file/d/18C1R1XPtCD8siLFdgYA-yR-teWY61Xsy/view?usp=drive_link)
- [Administration flow](https://drive.google.com/file/d/1whk9kUXY8xnFzyDIKguAPObQbWY_cad4/view?usp=drive_link)
- [Reception workflow](https://drive.google.com/file/d/1e3OPdMkw01cLJmYTmwnViQrHSMcEpX_C/view?usp=drive_link)
- [Mini game](https://drive.google.com/file/d/1DmjLOsuYcrr-0DYpHSzno8rts9Ps36qQ/view?usp=drive_link)

## What the platform provides

- Public room browsing, room details, date-based availability, and booking
- Customer authentication, registration, password recovery, and Google sign-in
- Booking payment flows through PayOS and VNPay
- Reception operations including booking review, guest registration, check-in, and check-out
- Administrative management of rooms, room types, amenities, passengers, invoices, and statistics
- Role-based access for customers, staff, and administrators
- JWT-protected integration endpoints for rooms, amenities, invoices, passengers, and statistics
- Email notifications through RabbitMQ-backed producer/consumer components
- Scheduled room-status refresh, check-in reminders, check-out reminders, and late check-out calculation
- Real-time notifications through SignalR
- Optional Redis-backed caching and distributed locking, with an in-memory fallback for local development
- QR-code generation for booking details

## Architecture at a glance

```text
Browser / API Client
        |
        v
HotelManagement.Web
  MVC controllers and Razor views
  Feature modules: Auth, Rooms, Payments, Invoices, Admin, API, Statistics
  Authentication, SignalR, Quartz jobs, RabbitMQ, Redis, file handling
        |
        v
HotelManagement.Infrastructure
  Entity Framework Core DbContext
  SQL Server models and migrations
        |
        v
     SQL Server

External integrations: Google OAuth, PayOS, VNPay, RabbitMQ, Redis
```

The solution uses a modular feature-oriented structure inside the web host. Persistence models, the EF Core context, and database migrations are kept in the Infrastructure project, while web workflows and integration endpoints are organized under the Web project’s `Modules` directory.

## Solution structure

```text
hotel-manager-booking/
├── README.md
├── .gitignore
└── Management Hotel 2025/
    ├── HotelManagement.sln
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
    │   │   ├── Statistics/
    │   │   └── WorkFile/
    │   ├── ViewModel/
    │   ├── Views/
    │   ├── wwwroot/
    │   ├── Program.cs
    │   └── appsettings.json
    ├── HotelManagement.Infrastructure/
    │   ├── Configuration/
    │   ├── Migrations/
    │   ├── Models/
    │   │   └── ManagermentHotelContext.cs
    │   └── HotelManagement.Infrastructure.csproj
    └── HotelManagement.Tests/
        ├── InvoiceApiTest.cs
        ├── TestValidationAuthen.cs
        └── HotelManagement.Tests.csproj
```

### Project responsibilities

| Project | Responsibility |
| --- | --- |
| `HotelManagement.Web` | ASP.NET Core MVC application, Razor UI, application services, API endpoints, authentication, payments, scheduled jobs, messaging, and real-time notifications |
| `HotelManagement.Infrastructure` | EF Core `ManagermentHotelContext`, SQL Server model configuration, persistence entities, and migrations |
| `HotelManagement.Tests` | xUnit tests for authentication validation and invoice API behavior |

## Technology stack

- .NET 8 and ASP.NET Core MVC
- Entity Framework Core 8 with SQL Server
- Razor Views, Bootstrap, jQuery, and client-side JavaScript
- Cookie authentication, JWT bearer authentication, and Google OAuth
- Quartz.NET for scheduled jobs
- SignalR for real-time notifications
- RabbitMQ.Client for asynchronous email processing
- StackExchange.Redis for optional caching and distributed locks
- PayOS and VNPay payment integrations
- QRCoder for QR-code generation
- xUnit and Moq for automated tests

## Prerequisites

- .NET 8 SDK
- SQL Server
- Visual Studio 2022 or another .NET-compatible IDE
- RabbitMQ if email messaging is enabled
- Redis if Redis caching/locking is enabled
- Credentials for Google OAuth, PayOS, and VNPay when those integrations are used

## Configuration

The application loads configuration from `appsettings.json`, environment variables, and an optional `.env` file. `.env` files and development settings are intentionally ignored by Git, so each developer must provide local values.

At minimum, configure the following sections for the features you need:

| Section | Used for |
| --- | --- |
| `ConnectionStrings:SQL` | SQL Server connection used by Entity Framework Core |
| `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience` | JWT creation and validation |
| `GoogleKeys:GoogleID`, `GoogleKeys:GoogleSecret` | Google authentication |
| `PayOS:ClientId`, `PayOS:ApiKey`, `PayOS:ChecksumKey` | PayOS checkout and webhook handling |
| `Vnpay:*` | VNPay URL, merchant settings, callback URL, and hash secret |
| `RabbitMQ:Host`, `RabbitMQ:UserName`, `RabbitMQ:Password`, `RabbitMQ:Queues:Email` | Email message delivery |
| `Redis:Enabled`, `Redis:Connection` | Optional Redis cache and lock services |

Use environment-variable nesting with double underscores when configuring outside JSON. For example:

```text
ConnectionStrings__SQL=Server=localhost;Database=HotelManagement;Trusted_Connection=True;TrustServerCertificate=True
Jwt__Key=replace-with-a-long-development-secret
Jwt__Issuer=hotel-manager-booking
Jwt__Audience=hotel-manager-booking-client
Redis__Enabled=false
```

Never commit passwords, signing keys, payment credentials, OAuth secrets, or production connection strings.

## Getting started

Run the commands below from the repository root.

### Restore and build

```powershell
dotnet restore "Management Hotel 2025\HotelManagement.sln"
dotnet build "Management Hotel 2025\HotelManagement.sln"
```

### Apply database migrations

The EF Core context and migrations are in `HotelManagement.Infrastructure`; the web project supplies the startup configuration:

```powershell
dotnet ef database update `
  --project "Management Hotel 2025\HotelManagement.Infrastructure\HotelManagement.Infrastructure.csproj" `
  --startup-project "Management Hotel 2025\HotelManagement.Web\HotelManagement.Web.csproj"
```

Review the migration history and confirm the target connection string before applying changes to a shared or production database.

### Run the application

```powershell
dotnet run --project "Management Hotel 2025\HotelManagement.Web\HotelManagement.Web.csproj"
```

The development launch profiles expose the application at:

- HTTPS: `https://localhost:7045`
- HTTP: `http://localhost:5299`

The default route opens the home introduction page. API and operational routes are implemented by the controllers under `HotelManagement.Web/Modules`.

## Testing

Run the test project or the complete solution with:

```powershell
dotnet test "Management Hotel 2025\HotelManagement.Tests\HotelManagement.Tests.csproj"
dotnet test "Management Hotel 2025\HotelManagement.sln"
```

The current test suite covers password validation and invoice API behavior using xUnit and Moq.

## Database model

The EF Core context currently includes entities for bookings, booking details, rooms, room types, users, guests, payments, invoices/orders, services, amenities, images, notifications, reviews, staff actions, tokens, and temporary PayOS bookings. Schema changes are tracked in:

```text
Management Hotel 2025/HotelManagement.Infrastructure/Migrations
```

## Project status

This project is under active development. Some naming and module paths retain their original historical names, while the solution continues to evolve toward clearer boundaries and more consistent conventions.

## Credits

- Software Engineer: Pham Trung Duc
- Development support and prototyping: ChatGPT, GitHub Copilot, and Pham Trung Duc

## License

No license file is currently included in the repository. Review and add an appropriate license before distributing the project publicly.

## Contact

For collaboration or feedback, contact [ptrungduc1011@gmail.com](mailto:ptrungduc1011@gmail.com).
