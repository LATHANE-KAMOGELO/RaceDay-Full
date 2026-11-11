# RaceDay — Event Management

RaceDay is a full-stack race-event management application for runners, walkers, and cycling events. It contains an ASP.NET Core REST API, an ASP.NET Core MVC website, SQL Server persistence, JWT authentication, and automated tests.
 
## Presentation

[Watch the RaceDay presentation on YouTube](https://youtu.be/FoI2yhLz5n8)

## Features

- Participant registration, login, and profile pages
- Organiser access to the event dashboard and event creation
- JWT-protected API endpoints
- SQL Server database access through Entity Framework Core migrations
- Swagger API documentation
- xUnit tests and GitHub Actions CI
- Docker Compose setup for the website, API, and SQL Server

## Solution layout

```text
RaceDay-Full-Stack/
├── .github/workflows/       # Continuous integration
├── RaceDay.API/             # ASP.NET Core API, data models, and migrations
├── RaceDay.API.Tests/       # Automated tests
├── RaceDay.MVC/             # ASP.NET Core MVC website
├── Dockerfile               # Builds either web project
├── compose.yaml             # API, website, and SQL Server services
├── .env.example             # Docker environment variable template
└── RaceDay.API.slnx         # Visual Studio solution
```

## Run with Docker

### Requirements

- Docker Desktop with Linux containers enabled
- At least 4 GB of memory available to Docker for SQL Server and both web apps

### Start the application

From the repository root, create a local environment file and edit its values:

```powershell
Copy-Item .env.example .env
notepad .env
```

Replace both example values in `.env` before starting the stack. Set `MSSQL_SA_PASSWORD` to a strong SQL Server password. Set `RACEDAY_JWT_KEY` to a private random key of at least 32 bytes. Keep `.env` on your computer; it is ignored by Git.

Build and start the website, API, and database:

```powershell
docker compose up --build
```

The API waits for SQL Server and applies the included Entity Framework migrations on startup. The first start can take a few minutes while Docker downloads the images and builds the projects. When the services are ready, open:

- Website: [http://localhost:7135](http://localhost:7135)
- Swagger API: [http://localhost:7195/swagger](http://localhost:7195/swagger)

The Docker setup uses HTTP for local demonstrations. Do not expose it directly to the public internet or use real passwords in this configuration.

To stop the containers, press **Ctrl+C** in the terminal running Compose, then run:

```powershell
docker compose down
```

The named SQL Server volume keeps the database when containers stop. To remove the containers **and permanently delete the Docker database volume**, run `docker compose down --volumes`.

If startup takes longer than expected, inspect the services with:

```powershell
docker compose ps
docker compose logs -f api
```

## Run locally with .NET

### Requirements

- .NET 10 SDK
- SQL Server
- Visual Studio 2022 or later, or the .NET CLI

1. Open `RaceDay.API.slnx`.
2. Configure the API connection string and JWT settings with User Secrets from the repository root:

   ```powershell
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=RaceDayDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True" --project RaceDay.API/RaceDay.API.csproj
   dotnet user-secrets set "Jwt:Key" "replace-with-a-random-secret-of-at-least-32-bytes" --project RaceDay.API/RaceDay.API.csproj
   dotnet user-secrets set "Jwt:Issuer" "RaceDay" --project RaceDay.API/RaceDay.API.csproj
   dotnet user-secrets set "Jwt:Audience" "RaceDayUsers" --project RaceDay.API/RaceDay.API.csproj
   ```

3. Restore, install EF tooling if needed, and apply the database migration:

   ```powershell
   dotnet restore RaceDay.API.slnx
   dotnet tool install --global dotnet-ef
   dotnet ef database update --project RaceDay.API/RaceDay.API.csproj
   ```

4. In Visual Studio, set **RaceDay.API** and **RaceDay.MVC** to **Start** under **Configure Startup Projects**. Leave the tests project set to **None**, then press **F5**. The API uses HTTPS port `7195`; the website uses HTTPS port `7135`.

   You can also run the projects in separate terminals:

   ```powershell
   dotnet run --project RaceDay.API/RaceDay.API.csproj --launch-profile https
   dotnet run --project RaceDay.MVC/RaceDay.MVC.csproj --launch-profile https
   ```

   Open [https://localhost:7135](https://localhost:7135) for the website. The API's Swagger page is at [https://localhost:7195/swagger](https://localhost:7195/swagger).

## Account roles

Public registration creates participant accounts. Organiser-only features require an account with the `Organiser` role.

## Tests

Run the test project from the repository root:

```powershell
dotnet test RaceDay.API.slnx
```

GitHub Actions runs restore, build, and tests for pushes and pull requests.
