# Contributing to Dental Clinic Project

Thanks for contributing.

## ✅ Before You Start

- Make sure the project builds locally.
- Read the existing architecture in `DentalClinicProject.API`, `DentalClinicProject.Core`, and `DentalClinicProject.Infrastructure`.
- Never commit secrets, connection strings with real credentials, API keys, or personal data.

## 🛠️ Local Setup

```powershell
dotnet restore .\DentalClinicProject.slnx
dotnet build .\DentalClinicProject.slnx
dotnet ef database update --project .\DentalClinicProject.Infrastructure --startup-project .\DentalClinicProject.API
dotnet run --project .\DentalClinicProject.API
```

## 🌿 Branching

- Create a feature branch from `main` (or the default branch).
- Use clear branch names such as:
  - `feature/appointment-filtering`
  - `fix/jwt-refresh-bug`
  - `docs/update-readme`

## ✍️ Coding Guidelines

- Follow the existing project structure and naming style.
- Keep controllers thin and move business logic into repositories/services where appropriate.
- Reuse DTOs, validators, and mappings instead of duplicating logic.
- Prefer small, focused pull requests.
- Add or update documentation when behavior changes.

## 🧪 Verification Checklist

Before submitting a pull request, confirm that:

- `dotnet build .\DentalClinicProject.slnx` succeeds
- any affected API endpoint was tested manually or automatically
- migrations are included if the database schema changed
- Swagger output remains usable for changed endpoints
- new secrets are stored outside source control

## 📝 Commit Messages

Use short, descriptive commit messages. Examples:

- `feat: add order payment confirmation endpoint`
- `fix: prevent duplicate cart items`
- `docs: improve setup instructions`

## 🔍 Pull Request Checklist

Include the following in your PR description:

1. **What changed**
2. **Why it changed**
3. **How it was tested**
4. **Any migration or config changes**

## 🛡️ Security and Privacy

Because this project uses authentication, email, phone verification, and external providers:

- avoid hardcoding credentials
- rotate compromised keys immediately
- validate input and authorization for any new endpoint
- review rate-limiting and abuse risks for auth-related features

## 💡 Documentation Contributions

Documentation improvements are welcome. If you change setup, configuration, routes, or deployment behavior, update `README.md` in the same PR when possible.
