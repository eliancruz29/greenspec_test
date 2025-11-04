# Implementation Plan: Industrial Automation Alert Service

## Overview
Build a complete full-stack application with **ASP.NET Core 9** backend and **Next.js 15** frontend for real-time sensor monitoring and alerting.

---

## **Phase 1: Backend Setup (ASP.NET Core 9)**

### 1.1 Solution Structure
Create .NET 9 solution with Clean Architecture structure:

```
backend/
├── src/
│   ├── GreenSpec.Domain/              # Core domain logic
│   ├── GreenSpec.Application/         # CQRS, DTOs, interfaces
│   ├── GreenSpec.Infrastructure/      # EF Core, SignalR, services
│   └── GreenSpec.Api/                 # Web API, controllers, hubs
├── tests/
│   └── GreenSpec.Tests.Unit/          # Unit tests
└── GreenSpec.sln
```

**Layer Dependencies:**
- `Domain` → No dependencies (pure domain logic)
- `Application` → Depends on `Domain`
- `Infrastructure` → Depends on `Application` and `Domain`
- `Api` → Depends on `Application` and `Infrastructure`

### 1.2 Core Domain Implementation

#### Entities
- **Config**: Sensor threshold configuration
  - `Id` (int)
  - `TempMax` (decimal)
  - `HumidityMax` (decimal)
  - `UpdatedAt` (DateTime)

- **Alert**: Alert records
  - `Id` (int)
  - `Type` (string - "Temperature" | "Humidity")
  - `Value` (decimal)
  - `Threshold` (decimal)
  - `CreatedAt` (DateTime)
  - `Status` (string - "Open" | "Acknowledged")

- **User**: Authentication
  - `Id` (int)
  - `Username` (string)
  - `PasswordHash` (string)

#### Value Objects
- **SensorReading**: Encapsulates temperature and humidity values
- **Threshold**: Encapsulates threshold validation logic

#### Domain Services
- **AlertGenerationService**: Business logic for threshold evaluation and alert creation

### 1.3 Database & Migrations

#### Configuration
- SQL Server with Entity Framework Core 9
- Connection string in `appsettings.json`
- DbContext: `ApplicationDbContext`

#### Migrations
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

#### Tables
| Table | Columns |
|-------|---------|
| **Configs** | Id, TempMax, HumidityMax, UpdatedAt |
| **Alerts** | Id, Type, Value, Threshold, CreatedAt, Status |
| **Users** | Id, Username, PasswordHash |

#### Seed Data
- Default configuration: TempMax = 30°C, HumidityMax = 80%
- Demo user: username = "admin", password = "admin123"

### 1.4 CQRS Implementation

Use **MediatR** for Command Query Responsibility Segregation pattern.

#### Commands
- **UpdateConfigCommand**: Update sensor thresholds
  - Handler: `UpdateConfigCommandHandler`
  - Validation: Ensure thresholds are positive values

- **AcknowledgeAlertCommand**: Acknowledge an alert
  - Handler: `AcknowledgeAlertCommandHandler`
  - Validation: Ensure alert exists and is in "Open" status

#### Queries
- **GetConfigQuery**: Retrieve current configuration
  - Handler: `GetConfigQueryHandler`
  - Returns: ConfigDto

- **GetAlertsQuery**: List alerts with filters
  - Parameters: Status (open/acknowledged), DateFrom, DateTo
  - Handler: `GetAlertsQueryHandler`
  - Returns: List<AlertDto>

#### DTOs
- `ConfigDto`
- `AlertDto`
- `SensorReadingDto`

### 1.5 Background Service

#### SensorSimulationService
Implemented as an `IHostedService` (BackgroundService).

**Behavior:**
1. Generate random sensor readings every 3-5 seconds
   - Temperature: 15°C - 40°C
   - Humidity: 40% - 95%
2. Compare readings against configured thresholds
3. When threshold exceeded:
   - Create Alert entity
   - Save to database
   - Publish real-time notification via SignalR

**Implementation:**
```csharp
public class SensorSimulationService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await SimulateSensorReading();
            await Task.Delay(Random.Shared.Next(3000, 5000), stoppingToken);
        }
    }
}
```

### 1.6 Authentication & API Endpoints

#### Authentication
- **JWT-based authentication**
- Token expiration: 24 hours
- Claims: UserId, Username

#### Endpoints

**AuthController**
- `POST /api/auth/login`
  - Body: `{ username, password }`
  - Returns: `{ token, expiresAt }`

**ConfigController** (Requires JWT)
- `GET /api/config`
  - Returns current configuration
- `PUT /api/config`
  - Body: `{ tempMax, humidityMax }`
  - Returns updated configuration

**AlertsController** (Requires JWT)
- `GET /api/alerts?status={open|ack}&from={date}&to={date}`
  - Returns filtered list of alerts
- `POST /api/alerts/{id}/ack`
  - Acknowledges specific alert
  - Returns updated alert

**SensorDataController** (Optional - for displaying recent readings)
- `GET /api/sensor-data`
  - Returns recent sensor readings

#### Swagger Documentation
- Configured with JWT bearer authentication
- Available at `/swagger`

### 1.7 SignalR Hub

#### AlertHub
Real-time communication hub for alert notifications.

**Methods:**
- Server → Client: `ReceiveAlert(AlertDto alert)`
  - Broadcasts new alerts to all connected clients

**Connection:**
- Endpoint: `/hubs/alerts`
- Requires JWT authentication

**Implementation:**
```csharp
public class AlertHub : Hub
{
    public async Task BroadcastAlert(AlertDto alert)
    {
        await Clients.All.SendAsync("ReceiveAlert", alert);
    }
}
```

### 1.8 Unit Tests

#### Test Coverage
- **Domain Services**: Threshold evaluation logic
- **Alert Generation**: Verify alerts are created when thresholds exceeded
- **CQRS Handlers**: Command and query handler logic
- **Validators**: Input validation logic

#### Framework
- xUnit
- Moq (for mocking)
- FluentAssertions (for assertions)

#### Example Tests
```csharp
[Fact]
public void ShouldCreateAlert_WhenTemperatureExceedsThreshold()
{
    // Arrange, Act, Assert
}

[Fact]
public async Task UpdateConfigCommand_ShouldUpdateThresholds()
{
    // Arrange, Act, Assert
}
```

---

## **Phase 2: Frontend Setup (Next.js 15)**

### 2.1 Project Initialization

```bash
npx create-next-app@latest frontend --typescript --tailwind --app
cd frontend
npm install @tanstack/react-query @microsoft/signalr
npm install axios zustand sonner
```

**Structure:**
```
frontend/
├── src/
│   ├── app/
│   │   ├── login/
│   │   │   └── page.tsx
│   │   ├── dashboard/
│   │   │   └── page.tsx
│   │   └── layout.tsx
│   ├── components/
│   │   ├── auth/
│   │   ├── dashboard/
│   │   └── ui/
│   ├── lib/
│   │   ├── api.ts
│   │   ├── auth.ts
│   │   └── signalr.ts
│   └── types/
│       └── index.ts
├── package.json
└── tailwind.config.ts
```

### 2.2 Authentication

#### Login Page (`/login`)
- Form with username and password fields
- Call `POST /api/auth/login`
- Store JWT token (localStorage or httpOnly cookies)
- Redirect to `/dashboard` on success

#### Auth State Management
- Zustand store for auth state
- `useAuth()` hook for accessing auth context
- Auto-redirect to `/login` if not authenticated

#### Protected Routes
- Middleware to check authentication
- Redirect unauthenticated users to `/login`

### 2.3 Dashboard Implementation

#### Layout
Responsive dashboard with:
1. Header with user info and logout button
2. Main content area with cards/sections
3. Sidebar navigation (optional)

#### Configuration Card
- Display current TempMax and HumidityMax
- Edit mode with input fields
- Save button to call `PUT /api/config`
- Loading and error states

#### Sensor Data Table
- Display recent simulated sensor readings
- Columns: Timestamp, Temperature, Humidity
- Auto-refresh with TanStack Query
- Visual indicators (colors) when values are near thresholds

#### Alerts Table
- Display alerts with columns: ID, Type, Value, Threshold, Created At, Status
- Filters:
  - Status dropdown (All / Open / Acknowledged)
  - Date range picker (From - To)
- Sort by Created At (newest first)
- Acknowledge button for open alerts
- Pagination or infinite scroll

#### Alert Acknowledgement
- Button on each open alert row
- Call `POST /api/alerts/{id}/ack`
- Optimistic update with TanStack Query
- Show success toast notification

### 2.4 Real-Time Integration

#### SignalR Client Setup
```typescript
const connection = new HubConnectionBuilder()
  .withUrl("http://localhost:5000/hubs/alerts", {
    accessTokenFactory: () => getAuthToken()
  })
  .withAutomaticReconnect()
  .build();
```

#### Receive Alerts
```typescript
connection.on("ReceiveAlert", (alert: AlertDto) => {
  // Show toast notification
  toast.error(`Alert: ${alert.type} exceeded threshold!`);
  // Invalidate alerts query to refresh list
  queryClient.invalidateQueries(['alerts']);
});
```

#### UI Indicators
- Toast notifications (using Sonner or similar)
- Badge with alert count
- Visual highlight on new alerts
- Sound notification (optional)

### 2.5 UI/UX

#### Responsive Design
- Mobile-first approach with TailwindCSS
- Breakpoints: sm, md, lg, xl
- Stacked layout on mobile, grid on desktop

#### Components
- Reusable UI components (Button, Card, Table, Input, etc.)
- Consistent styling with Tailwind utility classes
- Loading skeletons for async content
- Error boundaries for error handling

#### Accessibility
- ARIA labels for interactive elements
- Keyboard navigation support
- Focus indicators
- Semantic HTML

---

## **Phase 3: Docker & CI/CD**

### 3.1 Docker Setup

#### Backend Dockerfile
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/GreenSpec.Api/GreenSpec.Api.csproj", "src/GreenSpec.Api/"]
# ... copy other projects
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GreenSpec.Api.dll"]
```

#### Frontend Dockerfile
```dockerfile
FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM node:20-alpine
WORKDIR /app
COPY --from=build /app/.next ./.next
COPY --from=build /app/public ./public
COPY --from=build /app/package*.json ./
RUN npm ci --only=production
EXPOSE 3000
CMD ["npm", "start"]
```

#### docker-compose.yml
```yaml
version: '3.8'

services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong@Passw0rd
    ports:
      - "1433:1433"
    volumes:
      - sqldata:/var/opt/mssql

  backend:
    build:
      context: ./backend
      dockerfile: Dockerfile
    ports:
      - "5000:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=GreenSpecDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True
    depends_on:
      - sqlserver

  frontend:
    build:
      context: ./frontend
      dockerfile: Dockerfile
    ports:
      - "3000:3000"
    environment:
      - NEXT_PUBLIC_API_URL=http://localhost:5000
    depends_on:
      - backend

volumes:
  sqldata:
```

### 3.2 CI Integration (GitHub Actions)

#### .github/workflows/ci.yml
```yaml
name: CI

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  backend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup .NET 9
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '9.0.x'
      - name: Restore dependencies
        run: dotnet restore ./backend/GreenSpec.sln
      - name: Build
        run: dotnet build ./backend/GreenSpec.sln --no-restore
      - name: Test
        run: dotnet test ./backend/GreenSpec.sln --no-build --verbosity normal

  frontend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '20'
      - name: Install dependencies
        run: npm ci
        working-directory: ./frontend
      - name: Build
        run: npm run build
        working-directory: ./frontend
      - name: Lint
        run: npm run lint
        working-directory: ./frontend

  docker:
    runs-on: ubuntu-latest
    needs: [backend, frontend]
    steps:
      - uses: actions/checkout@v3
      - name: Build Docker images
        run: docker-compose build
```

---

## **Phase 4: Documentation & Testing**

### 4.1 Documentation

#### README.md
Comprehensive documentation including:
1. Project overview and features
2. Prerequisites (Node.js 20+, .NET 9 SDK, Docker)
3. Local development setup
4. Docker deployment instructions
5. Environment variables
6. API documentation links
7. Troubleshooting guide

#### API Documentation

**Swagger UI**
- Available at `http://localhost:5000/swagger`
- Interactive API exploration
- JWT authentication support

**.http Files**
Create `api-examples.http` with example requests:
```http
### Login
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "admin123"
}

### Get Config
GET http://localhost:5000/api/config
Authorization: Bearer {{token}}

### Update Config
PUT http://localhost:5000/api/config
Authorization: Bearer {{token}}
Content-Type: application/json

{
  "tempMax": 35,
  "humidityMax": 85
}

### Get Alerts
GET http://localhost:5000/api/alerts?status=open&from=2025-01-01&to=2025-12-31
Authorization: Bearer {{token}}

### Acknowledge Alert
POST http://localhost:5000/api/alerts/1/ack
Authorization: Bearer {{token}}
```

### 4.2 Final Testing

#### End-to-End Testing Checklist
- [ ] User can log in and receive JWT token
- [ ] Dashboard displays current configuration
- [ ] User can update sensor thresholds
- [ ] Background service generates sensor readings
- [ ] Alerts are created when thresholds exceeded
- [ ] Alerts appear in real-time via SignalR
- [ ] Toast notifications display for new alerts
- [ ] User can filter alerts by status and date
- [ ] User can acknowledge alerts
- [ ] All API endpoints are secured with JWT
- [ ] Docker deployment works correctly
- [ ] Database migrations run successfully
- [ ] Frontend is responsive on mobile devices

#### Load Testing (Optional)
- Test SignalR with multiple concurrent connections
- Verify background service performance
- Check database query performance with large datasets

---

## **Technology Stack Summary**

### Backend
| Technology | Purpose |
|------------|---------|
| ASP.NET Core 9 | Web API framework |
| Entity Framework Core 9 | ORM for SQL Server |
| MediatR | CQRS pattern implementation |
| SignalR | Real-time communication |
| JWT | Authentication |
| xUnit | Unit testing |
| FluentValidation | Input validation |
| Swagger/OpenAPI | API documentation |

### Frontend
| Technology | Purpose |
|------------|---------|
| Next.js 15 | React framework with App Router |
| TypeScript | Type safety |
| TailwindCSS | Styling |
| TanStack Query | Server state management |
| Zustand | Client state management |
| SignalR Client | Real-time communication |
| Axios | HTTP client |
| Sonner | Toast notifications |

### DevOps
| Technology | Purpose |
|------------|---------|
| Docker | Containerization |
| Docker Compose | Multi-container orchestration |
| GitHub Actions | CI/CD |
| SQL Server | Database |

---

## **Project Timeline**

### Day 1-2: Backend Foundation
- Set up solution structure
- Implement domain and application layers
- Configure EF Core and migrations
- Implement authentication

### Day 3: Backend Features
- Implement CQRS handlers
- Create background service
- Set up SignalR hub
- Write unit tests

### Day 4-5: Frontend Development
- Set up Next.js project
- Implement authentication flow
- Build dashboard components
- Integrate SignalR client

### Day 6: Integration & Docker
- End-to-end testing
- Create Dockerfiles
- Set up docker-compose
- Configure CI/CD

### Day 7: Documentation & Polish
- Write comprehensive README
- Create API documentation
- Final testing and bug fixes
- Code cleanup and optimization

---

## **Acceptance Criteria Checklist**

### Must-Have Features
- [x] All APIs secured with JWT (except login)
- [x] Background service simulates sensor data and generates alerts
- [x] SQL persistence with migrations
- [x] Functional Next.js app with:
  - [x] Login page
  - [x] Configuration editing
  - [x] Alert listing and filtering
  - [x] Alert acknowledgment
- [x] Real-time alert notifications via SignalR
- [x] Clean project structure (DDD, CQRS, Clean Architecture)
- [x] Comprehensive README with setup instructions
- [x] Complete Docker setup
- [x] Basic CI integration

### Non-Functional Requirements
- [x] Robust input validation and error handling
- [x] Unit tests for core business logic
- [x] Responsive and accessible UI
- [x] API documentation (Swagger + .http files)

---

## **Next Steps**

1. Execute this plan phase by phase
2. Test thoroughly at each stage
3. Commit code incrementally with clear messages
4. Update documentation as features are completed
5. Perform final end-to-end testing
6. Deploy and verify in Docker environment

---

**Document Version:** 1.0
**Last Updated:** 2025-11-03
**Status:** Ready for implementation
