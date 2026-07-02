# Hotel Manager Booking

Hotel Manager Booking is a multi-project ASP.NET Core solution for hotel reservation, front-desk operations, payment processing, and internal administration. The platform combines a web application, a partner-facing API, a shared data layer, and background workers to support both real-time interactions and scheduled operational tasks.

## Demo References

- Main flow: <https://drive.google.com/file/d/18C1R1XPtCD8siLFdgYA-yR-teWY61Xsy/view?usp=drive_link>
- Admin flow: <https://drive.google.com/file/d/1whk9kUXY8xnFzyDIKguAPObQbWY_cad4/view?usp=drive_link>
- Reception workflow: <https://drive.google.com/file/d/1e3OPdMkw01cLJmYTmwnViQrHSMcEpX_C/view?usp=drive_link>
- Mini game: <https://drive.google.com/file/d/1DmjLOsuYcrr-0DYpHSzno8rts9Ps36qQ/view?usp=drive_link>

## Overview

This repository is structured as an end-to-end hotel management platform rather than a single web application. It covers the full booking lifecycle, from room discovery and reservation to invoicing, notifications, operational monitoring, and external integration.

The solution is designed around the following objectives:

- Centralize reservation and room management workflows
- Support online payments and booking-related automation
- Expose API endpoints for external or partner integration
- Handle asynchronous and scheduled tasks outside the main request pipeline
- Provide a shared data model across application layers

## Solution Structure

The solution file is located at:

```text
Management Hotel 2025/Management Hotel 2025.sln
```

Projects included in the solution:

- `Management Hotel 2025`: main ASP.NET Core MVC application
- `API_BookingHotel`: REST API for system integration and external access
- `MyData`: shared Entity Framework Core models and data access artifacts
- `RabbitMQConsumer`: background worker for asynchronous processing
- `TEST`: unit test project

## Key Functional Areas

### Reservation Management

The platform supports core booking workflows including room discovery, availability-based selection, reservation creation, and booking follow-up. The codebase includes room search, room detail, booking, passenger, and invoice-related modules across both the MVC application and the API.

### Payment and Billing

The system integrates with online payment services such as PayOS and VNPay. Payment-related modules are present in the main application, alongside invoice handling and downstream notification workflows.

### Scheduling and Automation

Quartz.NET is used to run recurring operational jobs. Based on the current application startup configuration, the system includes scheduled tasks for:

- room-status refresh
- check-in reminders
- check-out reminders
- late check-out calculation

These tasks help move operational logic out of manual workflows and into predictable background execution.

### Realtime Communication

SignalR hubs are configured in the MVC application for realtime features. The current project setup includes hubs for notifications and game-related communication.

### Background Messaging

RabbitMQ is part of the architecture for asynchronous processing. The repository includes a dedicated worker service project and messaging-related modules in the main application.

### Caching and Coordination

Redis is used for caching and locking scenarios. The codebase includes Redis-based cache registration in the API and a lock service in the MVC application.

### API Integration

The API project exposes JWT-protected endpoints and integration modules related to rooms, amenities, passengers, invoices, files, and statistics. This separation allows external consumers to integrate without coupling directly to the web UI.

## Technical Stack

- `.NET 8`
- `ASP.NET Core MVC`
- `ASP.NET Core Web API`
- `Entity Framework Core`
- `SQL Server`
- `Quartz.NET`
- `SignalR`
- `RabbitMQ`
- `Redis`
- `JWT Authentication`
- `Cookie Authentication`
- `Google OAuth`
- `PayOS`
- `VNPay`
- `xUnit`
- `Moq`

## Architecture Summary

The repository follows a modular multi-project structure:

- The MVC application serves as the primary operational interface.
- The API project provides integration-oriented endpoints.
- The shared data project keeps the database model reusable across applications.
- The RabbitMQ worker handles background consumption separately from the web host.
- The test project isolates automated validation for selected business logic and API behavior.

At runtime, the application also relies on supporting infrastructure such as SQL Server, Redis, and RabbitMQ.

## Repository Layout

```text
hotel-manager-booking/
|-- README.md
|-- Management Hotel 2025/
|   |-- Management Hotel 2025.sln
|   |-- Management Hotel 2025/
|   |-- API_BookingHotel/
|   |-- MyData/
|   |-- RabbitMQConsumer/
|   |-- TEST/
```

## Prerequisites

To run the solution locally, prepare the following:

- .NET 8 SDK
- SQL Server
- Redis
- RabbitMQ
- Visual Studio 2022 or another .NET-compatible IDE

## Configuration

Application configuration is stored primarily in `appsettings.json` files within the individual projects.

Configuration areas currently used by the solution include:

- database connection strings
- JWT issuer, audience, and signing key
- Google authentication settings
- PayOS credentials
- API callback and integration URLs
- Redis connection settings

Before running the system in a new environment, review and update all machine-specific or secret values.

## Getting Started

### Restore dependencies

```powershell
dotnet restore "Management Hotel 2025\Management Hotel 2025.sln"
```

### Build the solution

```powershell
dotnet build "Management Hotel 2025\Management Hotel 2025.sln"
```

### Run the MVC application

```powershell
dotnet run --project "Management Hotel 2025\Management Hotel 2025\Management Hotel 2025.csproj"
```

### Run the API project

```powershell
dotnet run --project "Management Hotel 2025\API_BookingHotel\API_BookingHotel.csproj"
```

### Run the background worker

```powershell
dotnet run --project "Management Hotel 2025\RabbitMQConsumer\RabbitMQConsumer.csproj"
```

## Testing

Execute the test suite with:

```powershell
dotnet test "Management Hotel 2025\Management Hotel 2025.sln"
```

The current test project contains examples for authentication validation and invoice API behavior.

## Credits

- Software Engineer: Pham Trung Duc
- Frontend support and prototyping: ChatGPT, Copilot, Pham Trung Duc

## Contact

For collaboration or feedback:

`ptrungduc1011@gmail.com`

## Notes

- This project is currently under active development.
- No explicit license file is included in the repository at this time.
- Some image assets used by the project may originate from external sources and should be reviewed before redistribution.
