# Dental Clinic Project

A layered **ASP.NET Core Web API** for managing dental clinic operations and a small clinic store. The solution includes authentication, profile management, appointments, services, products, cart handling, orders, payments, ratings, and admin workflows.

## ✨ Highlights

- **JWT authentication** with refresh-token flow
- **Role-based authorization** for `Admin`, `Doctor`, `Patient`, `User`, and `Delivery`
- **Optional Google sign-in** support
- **Email and phone verification** integrations
- **Appointment and dental service management**
- **Product catalog, cart, orders, and payment handling**
- **Ratings** for doctors, products, and appointments
- **Redis integration** for helper services and token-related workflows
- **Swagger/OpenAPI**, rate limiting, request sanitization, and global exception handling

## 🏗️ Solution Structure

| Project | Responsibility |
|---|---|
| `DentalClinicProject.API` | API startup, controllers, middleware, mappings, Swagger |
| `DentalClinicProject.Core` | Entities, DTOs, enums, interfaces, validators, view models, seed data |
| `DentalClinicProject.Infrastructure` | EF Core context, repositories, services, logging, migrations, dependency injection |

## 🧰 Tech Stack

- **.NET 10** (`net10.0`)
- **ASP.NET Core Web API**
- **Entity Framework Core + SQL Server**
- **ASP.NET Core Identity + JWT Bearer**
- **AutoMapper**
- **FluentValidation**
- **StackExchange.Redis**
- **MailKit** and **Twilio**
- **Swagger / OpenAPI**

## 📦 Main Functional Areas

- **Auth**: register, login, refresh token, logout, email verification, phone verification
- **External Login**: Google login flow
- **Appointments**: create, update, cancel, query by doctor/patient
- **Services**: CRUD operations for clinic services
- **Products**: catalog listing and admin CRUD
- **Cart**: add/remove/clear products
- **Orders**: create orders, checkout cart, shipping and payment status updates
- **Profile**: get, update, and delete profile
- **Rates**: manage ratings for doctors, products, and appointments
- **Admin Management**: create, update, and delete admin accounts

## ⚙️ Configuration

Update `DentalClinicProject.API/appsettings.json` or use **User Secrets** / environment variables for sensitive values:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ClinicDatabase;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;",
    "Redis": "localhost:6379"
  },
  "JWT": {
    "Key": "<your-secret-key>",
    "Issuer": "Dental_Clinic_Project",
    "Audience": "Dental_Clinic_Project"
  },
  "Mail": {
    "Key": "<mail-api-key>",
    "Username": "<mail-username>"
  },
  "Twilio": {
    "AccountSID": "<sid>",
    "AuthToken": "<token>",
    "PhoneNumber": "<number>",
    "VerifyServiceSID": "<verify-sid>"
  },
  "Google": {
    "ClientId": "<client-id>",
    "ClientSecret": "<client-secret>"
  }
}
```

> Do **not** commit real secrets to source control.

## 🚀 Getting Started

### Prerequisites

1. **.NET 10 SDK**
2. **SQL Server**
3. **Redis**
4. Optional: valid **SMTP / SendGrid**, **Twilio**, and **Google OAuth** credentials

### 1) Restore and build

```powershell
dotnet restore .\DentalClinicProject.slnx
dotnet build .\DentalClinicProject.slnx
```

### 2) Apply database migrations

```powershell
dotnet ef database update --project .\DentalClinicProject.Infrastructure --startup-project .\DentalClinicProject.API
```

### 3) Run the API

```powershell
dotnet run --project .\DentalClinicProject.API
```

By default, the development launch settings expose the API on:

- `https://localhost:7114`

Swagger UI is available in **Development** mode at:

- `https://localhost:7114/swagger`

## 🔐 Security Notes

The API already includes several production-oriented safeguards:

- JWT validation
- role-based authorization
- request rate limiting
- HTML sanitization
- exception-handling middleware
- token blacklist middleware

You should still review all security settings before deploying to production.

## 🌱 Seed Data

The project contains seed helpers under `DentalClinicProject.Core/Seeding/` for roles and sample data. Roles are seeded automatically on startup.

## 🤝 Contributing

Please read [`CONTRIBUTING.md`](./CONTRIBUTING.md) before opening a pull request.

## 📄 License

This repository is licensed under the **MIT License**. See [`LICENSE`](./LICENSE) for details.
