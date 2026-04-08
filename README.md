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

### Development Setup

The API is configured via `Api/appsettings.json` for non-sensitive settings. **Sensitive values like JWT keys use .NET User Secrets**:

1. User secrets are already initialized for the project
2. A secure JWT key has been generated automatically
3. To view/modify secrets: `dotnet user-secrets list --project Api`
4. To set a new key: `dotnet user-secrets set "Jwt:Key" "your-secure-key-here" --project Api`

### Production Setup (Home Server on Linux)

**Never commit secrets to source control.** For a Linux server:

1. **Generate a production JWT key:**
   ```bash
   openssl rand -base64 32
   ```
   Copy the output - this is your production key.

2. **Edit the service file:**
   - Open `perpendicularity.service`
   - Replace `YOUR_USERNAME` with your Linux username (run `whoami` to find it)
   - Replace `CHANGE_THIS_TO_YOUR_SECRET_KEY` with your generated key
   - Save the file (**don't commit it!**)

3. **Install and start the service:**
   ```bash
   # Copy service file to systemd
   sudo cp perpendicularity.service /etc/systemd/system/
   
   # Reload systemd to recognize the new service
   sudo systemctl daemon-reload
   
   # Enable auto-start on boot
   sudo systemctl enable perpendicularity
   
   # Start the service now
   sudo systemctl start perpendicularity
   
   # Check if it's running
   sudo systemctl status perpendicularity
   ```

4. **View logs:**
   ```bash
   sudo journalctl -u perpendicularity -f
   ```

5. **Stop/restart when needed:**
   ```bash
   sudo systemctl stop perpendicularity
   sudo systemctl restart perpendicularity
   ```

**For cloud hosting:** Use your platform's secrets management (Azure Key Vault, AWS Secrets Manager, etc.)

### JWT Configuration

- **Issuer**: `Perpendicularity.Api` (identifies your API)
- **Audience**: `Perpendicularity.Ui` (identifies your frontend)
- **Key**: Automatically generated 32+ character secret (stored in user secrets for dev)
- **ExpiryInHours**: Token lifetime (default: 1 hour)

## Configuration

- **API**: Configured in `Api/appsettings.json` (JWT settings, port 63000)
- **Database**: MongoDB on localhost:63002 (auto-created on first run)

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
