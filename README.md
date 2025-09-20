# Organize-Tool

[![MIT License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-6.0%2B-blue)](https://dotnet.microsoft.com/)
[![GitHub Repo](https://img.shields.io/badge/GitHub-Repository-blue?logo=github)](https://github.com/islammohmed/Organize-Tool)


Organize-Tool is a .NET-based API project designed to help organizations manage projects, tasks, users, and time entries efficiently. It integrates with Clockify for time tracking and provides features for data synchronization and reporting.

## GitHub Repository

- **Source Code:** [github.com/islammohmed/Organize-Tool](https://github.com/islammohmed/Organize-Tool)
- **Issues & Feature Requests:** Use the [GitHub Issues](https://github.com/islammohmed/Organize-Tool/issues) page to report bugs or request features.

## Features
- Project, Task, and User management
- Time entry tracking and synchronization (Clockify/Toggl)
- Assignment management
- Data export and reporting (monthly reports)
- RESTful API endpoints
- Entity Framework Core for data access
- Repository and Unit of Work patterns

## Project Structure
- `ClockifyData.API/` - ASP.NET Core Web API layer (controllers, configuration)
- `ClockifyData.Application/` - Application logic, DTOs, services, interfaces
- `ClockifyData.Domain/` - Domain entities and models
- `ClockifyData.Infrastructure/` - Data access, repositories, migrations

## Getting Started
### Prerequisites
- .NET 6.0 or later
- SQL Server (or update connection string for your DB)

### Setup
1. Clone the repository:
   ```pwsh
   git clone https://github.com/islammohmed/Organize-Tool.git
   ```
2. Navigate to the project folder:
   ```pwsh
   cd Organize-Tool
   ```
3. Update `appsettings.json` with your database connection string and Clockify API credentials.
4. Apply migrations:
   ```pwsh
   dotnet ef database update --project ClockifyData.Infrastructure
   ```
5. Run the API:
   ```pwsh
   dotnet run --project ClockifyData.API
   ```

## API Endpoints
- `/api/projects` - Manage projects
- `/api/tasks` - Manage tasks
- `/api/users` - Manage users
- `/api/timeentries` - Track and sync time entries
- `/api/assignments` - Manage assignments
- `/api/export` - Export reports


## Contributing

Contributions are welcome! Please fork the repo and submit a pull request via GitHub. For major changes, open an issue first to discuss what you would like to change.

## Maintainers & Contact

- **Maintainer:** [islammohmed](https://github.com/islammohmed)
- For questions, open an issue or contact via GitHub.

