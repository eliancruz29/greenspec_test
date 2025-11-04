# GreenSpec - Industrial Automation Alert Service

A real-time industrial automation monitoring system built with .NET 8, Next.js 15, and SignalR for sensor data monitoring and alert management.

## Features

- **Real-time Monitoring**: Live sensor data simulation (Temperature & Humidity)
- **Threshold Management**: Configure custom alert thresholds
- **Alert System**: Automatic alert generation when readings exceed thresholds
- **Real-time Notifications**: SignalR-powered WebSocket notifications
- **Alert Management**: View, filter, and acknowledge alerts
- **Modern Architecture**: Clean Architecture, CQRS, DDD principles
- **Type-Safe**: Full TypeScript support on frontend
- **Containerized**: Complete Docker setup with docker-compose

## Tech Stack

### Backend (.NET 8)

- **Framework**: ASP.NET Core 8 with Minimal APIs
- **Database**: SQL Server with Entity Framework Core 8
- **Authentication**: JWT Bearer tokens with BCrypt password hashing
- **Real-time**: SignalR for WebSocket communication
- **Patterns**: Clean Architecture, CQRS (MediatR), DDD
- **Validation**: FluentValidation
- **Documentation**: Swagger/OpenAPI

### Frontend (Next.js 15)

- **Framework**: Next.js 15 with App Router
- **Language**: TypeScript
- **Styling**: TailwindCSS
- **State Management**: TanStack Query (React Query)
- **HTTP Client**: Axios
- **Real-time**: SignalR JavaScript Client
- **Notifications**: Browser Notification API

### DevOps

- **Containerization**: Docker & Docker Compose
- **CI/CD**: GitHub Actions
- **Testing**: xUnit with FluentAssertions and Moq (82 unit tests)

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                         Frontend                             │
│  Next.js 15 + TypeScript + TailwindCSS + SignalR Client    │
└────────────────────────┬────────────────────────────────────┘
                         │ HTTPS/WSS
┌────────────────────────┴────────────────────────────────────┐
│                      Backend API                             │
│              ASP.NET Core 8 Minimal API                     │
├──────────────────────────────────────────────────────────────┤
│  Application Layer (CQRS + MediatR + FluentValidation)     │
├──────────────────────────────────────────────────────────────┤
│  Domain Layer (Entities + Value Objects + Services)         │
├──────────────────────────────────────────────────────────────┤
│  Infrastructure (EF Core + SignalR + Background Service)    │
└────────────────────────┬────────────────────────────────────┘
                         │
                    ┌────┴─────┐
                    │ SQL Server│
                    └──────────┘
```

## Quick Start

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 20+](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (for containerized setup)

### Option 1: Docker Compose (Recommended)

1. Clone the repository:

```bash
git clone <repository-url>
cd greenspec_test
```

2. Start all services:

```bash
docker-compose up -d
```

3. Wait for services to start (approximately 30 seconds)

4. Access the application:
   - **Frontend**: http://localhost:3000
   - **Backend API**: http://localhost:5001
   - **Swagger UI**: http://localhost:5001/swagger

5. Login with default credentials:
   - Username: `admin`
   - Password: `admin123`

### Option 2: Manual Setup

#### Backend

1. Start SQL Server:

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" \
  -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
```

2. Navigate to backend and run:

```bash
cd backend/src/GreenSpec.Api
dotnet run
```

3. The API will be available at `https://localhost:5001`

#### Frontend

1. Install dependencies:

```bash
cd frontend
npm install
```

2. Create `.env.local` file:

```env
NEXT_PUBLIC_API_URL=https://localhost:5001/api
NEXT_PUBLIC_SIGNALR_URL=wss://localhost:5001/hubs/alerts
```

3. Run the development server:

```bash
npm run dev
```

4. Open http://localhost:3000

## API Endpoints

### Authentication

- `POST /api/auth/login` - Login and get JWT token

### Configuration

- `GET /api/config` - Get current threshold configuration
- `PUT /api/config` - Update threshold configuration

### Alerts

- `GET /api/alerts` - Get all alerts (supports filters: status, from, to)
- `POST /api/alerts/{id}/ack` - Acknowledge an alert

### SignalR Hub

- `WS /hubs/alerts` - Real-time alert notifications

## Testing

### Backend Unit Tests

Run all 82 unit tests:

```bash
cd backend
dotnet test
```

### API Testing with .http Files

Use the provided [backend/api-tests.http](backend/api-tests.http) file with REST Client extension in VS Code:

1. Login to get token
2. Test configuration endpoints
3. Test alert endpoints
4. Test error scenarios

## Project Structure

```
greenspec_test/
├── backend/
│   ├── src/
│   │   ├── GreenSpec.Api/             # Minimal API endpoints
│   │   ├── GreenSpec.Application/     # CQRS, DTOs, Validators
│   │   ├── GreenSpec.Domain/          # Entities, Value Objects
│   │   └── GreenSpec.Infrastructure/  # EF Core, SignalR, Services
│   ├── tests/
│   │   └── GreenSpec.Tests.Unit/      # 82 unit tests
│   ├── Dockerfile
│   └── api-tests.http
├── frontend/
│   ├── app/                            # Next.js App Router pages
│   ├── components/                     # React components
│   ├── lib/                            # API, auth, SignalR utilities
│   ├── Dockerfile
│   └── package.json
├── .github/
│   └── workflows/
│       └── ci.yml                      # GitHub Actions CI/CD
├── docker-compose.yml
├── planning.md                         # Detailed implementation plan
├── PROGRESS.md                         # Progress tracking
└── README.md                           # This file
```

## Configuration

### Backend (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=GreenSpecDb;..."
  },
  "Jwt": {
    "Secret": "YourSecretKey",
    "Issuer": "GreenSpec",
    "Audience": "GreenSpec"
  },
  "Cors": {
    "Origins": "http://localhost:3000"
  }
}
```

### Frontend (.env.local)

```env
NEXT_PUBLIC_API_URL=https://localhost:5001/api
NEXT_PUBLIC_SIGNALR_URL=wss://localhost:5001/hubs/alerts
```

## Features in Detail

### Sensor Simulation

- Background service generates random sensor readings every 3-5 seconds
- Temperature: 15-40°C
- Humidity: 30-90%
- Automatic alert generation when thresholds are exceeded

### Alert Management

- **Status Filtering**: View all, open, or acknowledged alerts
- **Date Filtering**: Filter alerts by date range
- **One-Click Acknowledgment**: Mark alerts as acknowledged
- **Real-time Updates**: Automatic table refresh when new alerts arrive

### Real-time Notifications

- SignalR WebSocket connection
- Browser notifications for new alerts
- Automatic reconnection on disconnect
- React Query cache invalidation for instant UI updates

## Development

### Backend Development

```bash
cd backend/src/GreenSpec.Api
dotnet watch run
```

### Frontend Development

```bash
cd frontend
npm run dev -- --turbopack
```

### Database Migrations

```bash
cd backend/src/GreenSpec.Api
dotnet ef migrations add MigrationName --project ../GreenSpec.Infrastructure/GreenSpec.Infrastructure.csproj
dotnet ef database update
```

## CI/CD Pipeline

The GitHub Actions workflow automatically:

1. Builds and tests the backend (.NET 8)
2. Runs 82 unit tests with code coverage
3. Builds Docker images for backend and frontend
4. Runs integration tests with docker-compose
5. Validates ESLint and builds frontend

## Security

- **Authentication**: JWT Bearer tokens with secure password hashing (BCrypt)
- **Authorization**: All endpoints except login require authentication
- **CORS**: Configured to allow only specified origins
- **SQL Injection**: Protected via EF Core parameterized queries
- **XSS**: Protected via React's built-in escaping

## Performance

- **Background Processing**: Sensor simulation runs independently
- **Efficient Queries**: EF Core with proper indexing
- **Real-time**: SignalR with automatic reconnection
- **Caching**: React Query with smart cache invalidation
- **Docker**: Multi-stage builds for optimized image sizes

## Troubleshooting

### Backend won't start

- Ensure SQL Server is running
- Check connection string in appsettings.json
- Verify .NET 8 SDK is installed

### Frontend can't connect to backend

- Check NEXT_PUBLIC_API_URL in .env.local
- Ensure backend is running on the correct port
- Verify CORS settings allow frontend origin

### SignalR not connecting

- Check browser console for WebSocket errors
- Verify NEXT_PUBLIC_SIGNALR_URL is correct
- Ensure JWT token is valid

### Docker Compose issues

- Run `docker-compose down -v` to clean volumes
- Check Docker Desktop is running
- Verify ports 3000, 5001, and 1433 are available

## License

This project is for educational and demonstration purposes.

## Author

Built with Clean Architecture principles and modern .NET/React best practices.
