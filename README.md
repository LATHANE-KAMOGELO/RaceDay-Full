# RaceDay

RaceDay is a race-event management application with an ASP.NET Core API and an MVC website. Participants can create accounts and maintain a race profile. Organisers can manage events.

## Presentation

[Watch the RaceDay presentation](https://youtu.be/FoI2yhLz5n8)

## Projects

- **RaceDay.API** — REST API, JWT authentication, participant and event endpoints, SQL Server persistence, and EF Core migrations.
- **RaceDay.MVC** — the website, including account, participant, organiser, and event pages.
- **RaceDay.API.Tests** — automated tests for API request models and the MVC API client.

## Requirements

- .NET 10 SDK
- SQL Server
- Visual Studio 2022 or later, or the .NET CLI

## Local setup

1. Clone the repository and open `RaceDay.API.slnx`.
2. Create or select a SQL Server database and configure the API with .NET User Secrets. The project already has a User Secrets ID, so these commands can be run from the repository root:

   ```powershell
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=RaceDayDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True" --project RaceDay.API/RaceDay.API.csproj
   dotnet user-secrets set "Jwt:Key" "replace-with-a-random-secret-of-at-least-32-bytes" --project RaceDay.API/RaceDay.API.csproj
   dotnet user-secrets set "Jwt:Issuer" "RaceDay" --project RaceDay.API/RaceDay.API.csproj
   dotnet user-secrets set "Jwt:Audience" "RaceDayUsers" --project RaceDay.API/RaceDay.API.csproj
   ```

   Use a private, random JWT key for your own development environment. Do not commit secrets.

3. Restore packages and apply the included database migration:

   ```powershell
   dotnet restore RaceDay.API.slnx
   dotnet tool install --global dotnet-ef
   dotnet ef database update --project RaceDay.API/RaceDay.API.csproj
   ```

4. Start the API and MVC website together. In Visual Studio, right-click the solution, choose **Configure Startup Projects**, select **Multiple startup projects**, set **RaceDay.API** and **RaceDay.MVC** to **Start**, and leave **RaceDay.API.Tests** set to **None**. Then press **F5**. The API uses HTTPS port `7195`; the website uses HTTPS port `7135`.

   Alternatively, start each project in its own terminal with the `https` launch profile:

   ```powershell
   dotnet run --project RaceDay.API/RaceDay.API.csproj --launch-profile https
   dotnet run --project RaceDay.MVC/RaceDay.MVC.csproj --launch-profile https
   ```

   Open the website at [https://localhost:7135](https://localhost:7135). Accept the local ASP.NET development certificate if your browser asks.

## Account roles

Public registration creates participant accounts. Organiser-only features require an account with the `Organiser` role.

## Tests

Run the test project from the repository root:

```powershell
dotnet test RaceDay.API.slnx
```

GitHub Actions runs restore, build, and tests for pushes and pull requests.
