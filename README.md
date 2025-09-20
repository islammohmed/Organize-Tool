# Organize-Tool

[![MIT License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-6.0%2B-blue)](https://dotnet.microsoft.com/)
[![GitHub Repo](https://img.shields.io/badge/GitHub-Repository-blue?logo=github)](https://github.com/islammohmed/Organize-Tool)


Organize-Tool is a comprehensive .NET-based API project designed to help organizations track and manage projects, tasks, users, and time entries efficiently. It features seamless integration with time tracking platforms like Clockify and Toggl, allowing for automated synchronization of time entries. The application enables teams to monitor project progress, manage task assignments, track time spent, and generate detailed reports for better resource planning and billing.

## GitHub Repository

- **Source Code:** [github.com/islammohmed/Organize-Tool](https://github.com/islammohmed/Organize-Tool)
- **Issues & Feature Requests:** Use the [GitHub Issues](https://github.com/islammohmed/Organize-Tool/issues) page to report bugs or request features.

## Features
- **Project, Task, and User Management**: Create and manage projects, tasks and user assignments
- **Time Entry Tracking**: Record working hours with start and end times for tasks
- **Multi-Provider Time Synchronization**: Supports both Clockify and Toggl integrations
- **Assignment Management**: Assign tasks to users with estimated hours
- **Flexible Data Export**: Export time entries as CSV reports (monthly, custom date range)
- **RESTful API Architecture**: Well-defined API endpoints for all operations
- **Robust Data Access**: Entity Framework Core with Repository and Unit of Work patterns
- **Batch Processing**: Support for batch synchronization of time entries across providers

## Project Structure
This project follows a clean architecture approach with clear separation of concerns:

- **`ClockifyData.API/`** - ASP.NET Core Web API layer
  - Controllers for Projects, Tasks, Users, TimeEntries, Sync, Export
  - Configuration and dependency injection setup
  - API endpoint definitions

- **`ClockifyData.Application/`** - Application business logic
  - DTOs for data transfer and API requests/responses
  - Services implementing business logic
  - Interfaces defining contracts for services and repositories
  - Factory patterns for TimeEntry synchronization
  - Mapping utilities for entity-to-DTO conversions

- **`ClockifyData.Domain/`** - Core domain entities and models
  - Project, Task, TimeEntry, User, and UserTask entities
  - Clean domain model with proper relationships
  - Entity configurations and validation rules

- **`ClockifyData.Infrastructure/`** - Data access and persistence
  - Entity Framework DbContext and configuration
  - Repository implementations for data access
  - Unit of Work pattern implementation
  - Database migrations and seeding
  - External service integrations

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

3. Update the database connection string and Clockify/Toggl API credentials in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=ClockifyDataDb;User=root;Password=yourpassword;"
     },
     "Clockify": {
       "ApiKey": "your-clockify-api-key",
       "WorkspaceId": "your-workspace-id"
     }
   }
   ```

4. Install the Entity Framework Core tools if you haven't already:
   ```pwsh
   dotnet tool install --global dotnet-ef
   ```

5. Apply migrations to create the database:
   ```pwsh
   dotnet ef database update --project ClockifyData.Infrastructure --startup-project ClockifyData.API
   ```

6. Build the solution:
   ```pwsh
   dotnet build
   ```

7. Run the API:
   ```pwsh
   dotnet run --project ClockifyData.API
   ```

8. Access the API at `https://localhost:5001` or `http://localhost:5000`

## API Endpoints

### Projects
- `GET /api/projects` - Get all projects
- `GET /api/projects/{id}` - Get project by ID
- `POST /api/projects` - Create a new project
- `PUT /api/projects/{id}` - Update project
- `DELETE /api/projects/{id}` - Delete project

### Tasks
- `GET /api/tasks` - Get all tasks
- `GET /api/tasks/{id}` - Get task by ID
- `GET /api/tasks/project/{projectId}` - Get tasks by project
- `POST /api/tasks` - Create a new task
- `PUT /api/tasks/{id}` - Update task
- `DELETE /api/tasks/{id}` - Delete task

### Users
- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `POST /api/users` - Create a new user
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user

### Time Entries
- `GET /api/timeentries` - Get time entries (filter by user or date)
- `GET /api/timeentries/all` - Get all time entries
- `GET /api/timeentries/user/{userId}` - Get time entries by user
- `GET /api/timeentries/range?from={date}&to={date}` - Get entries in date range
- `POST /api/timeentries` - Create a new time entry

### Sync
- `POST /api/sync/clockify` - Sync time entries to Clockify
- `POST /api/sync/toggl` - Sync time entries to Toggl
- `POST /api/timeentrysync/{provider}` - Sync to specified provider

### Export
- `GET /api/export/report/csv?from={date}&to={date}` - Export time entries to CSV
- `GET /api/export/current-month/csv` - Export current month's entries to CSV

### Assignments
- `GET /api/assignments` - Get all task assignments
- `POST /api/assignments` - Assign task to user
- `DELETE /api/assignments/{id}` - Remove assignment


## Contributing

Contributions are welcome! Please fork the repo and submit a pull request via GitHub. For major changes, open an issue first to discuss what you would like to change.

## Architecture

The project follows a clean, layered architecture with:

### Domain Layer
- Core business entities
- Entity relationships and validations
- Business rules and constraints

### Application Layer
- Business logic and services
- DTOs for data transfer
- Interface definitions
- Mapping profiles

### Infrastructure Layer
- Data persistence via Entity Framework Core
- Repository implementations
- External service integrations
- Database migrations

### API Layer
- REST controllers
- Request/response handling
- Authentication and authorization
- API documentation

## Data Model

### Key Entities
- **User**: System users who can be assigned to tasks and track time
- **Project**: Container for tasks with user ownership
- **Task**: Units of work with time estimation, linked to projects
- **TimeEntry**: Records of time spent on tasks by users
- **UserTask**: Assignments linking users to tasks

## Technical Stack

- **Framework**: .NET 6.0+
- **ORM**: Entity Framework Core
- **API**: ASP.NET Core Web API
- **Database**: SQL Server (configurable)
- **Design Patterns**: Repository, Unit of Work, Factory
- **External Integrations**: Clockify API, Toggl API

## Maintainers & Contact

- **Maintainer:** [islammohmed](https://github.com/islammohmed)
- For questions, open an issue or contact via GitHub.