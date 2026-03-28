# Perpendicularity

A full-stack web application built with .NET 9.0 and Vue.js, featuring JWT authentication and MongoDB data storage.

## Architecture

- **Backend**: ASP.NET Core Web API (.NET 9.0)
- **Frontend**: Vue 3 + TypeScript + Vite
- **Database**: MongoDB
- **Authentication**: JWT Bearer tokens

## Project Structure

```
├── Api/                    # ASP.NET Core Web API project
├── Core/                   # Domain layer
├── Infrastructure/         # Third-party libraries such as Mongo
├── Test/                   # Unit tests
├── Ui/                     # Vue.js frontend application
```

## Prerequisites

Before running the application, ensure you have the following installed:

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 18+](https://nodejs.org/) and npm
- [Docker](https://www.docker.com/get-started) and Docker Compose

## Getting Started

### First Time Setup

1. **Start Containers**: Run `docker-compose up -d` in the repository root
2. **Install UI dependencies**: Run `npm install` in the `Ui` directory

### Running the Application

- **API**: Use the VS Code launch configuration or run `dotnet run --project Api/Api.csproj`. Available at **http://localhost:63000**
- **UI**: Run `npm run dev` in the `Ui` directory. Available at **http://localhost:63003**

## Development Workflow

- **Regenerate API Client**: Run `npm run build-perpendicularity-api-client` in `Ui` after changing API controllers
- **Run Tests**: `dotnet test`
- **API Docs**: Available at http://localhost:63000/swagger when API is running

## Configuration

- **API**: Configured in `Api/appsettings.json` (JWT settings, port 63000)
- **Database**: MongoDB on localhost:63002 (auto-created on first run)

> **Note**: Change the JWT key for production environments.

## Production Build

- **API**: `dotnet publish Api/Api.csproj -c Release -o ./publish`
- **UI**: `npm run build` in `Ui` directory (output in `Ui/dist/`)

## Tech Stack Details

### Backend
- **ASP.NET Core 9.0** - Web API framework
- **MongoDB Driver** - Database access
- **MediatR** - CQRS pattern implementation
- **JWT Bearer Authentication** - User authentication
- **Swagger/OpenAPI** - API documentation

### Frontend
- **Vue 3** - Progressive JavaScript framework
- **TypeScript** - Type-safe JavaScript
- **Vite** - Build tool and dev server
- **PrimeVue** - UI component library
- **Tailwind CSS** - Utility-first CSS framework
- **Vue Router** - Client-side routing
- **NSwag** - API client generation

## Troubleshooting

- **Port conflicts**: Change ports in `appsettings.json` (API) or `docker-compose.yml` (MongoDB)
- **MongoDB connection failed**: Check `docker ps` to ensure container is running
- **API client errors**: Regenerate the client with `npm run build-perpendicularity-api-client`
