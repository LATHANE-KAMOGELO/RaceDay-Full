
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using RaceDay.API.Data;
using RaceDay.API.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add controllers.
builder.Services.AddControllers();

// Register authentication service.
builder.Services.AddScoped<AuthService>();

// Connect to SQL Server.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure JWT authentication.
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT key is missing.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("JWT issuer is missing.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("JWT audience is missing.");

if (Encoding.UTF8.GetByteCount(jwtKey) < 32)
{
    throw new InvalidOperationException(
        "JWT key must be at least 32 bytes long.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
        JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme =
        JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,

        ValidateAudience = true,
        ValidAudience = jwtAudience,

        ValidateLifetime = true,

        ValidateIssuerSigningKey = true,

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey)),

        ClockSkew = TimeSpan.Zero
    };
});

// Configure authorization.
builder.Services.AddAuthorization();

// Configure Swagger and JWT Bearer authentication.
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token here."
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});

// Build application.
var app = builder.Build();

// Docker Compose can request migrations after its SQL Server service starts.
// Keep this opt-in so local and production startup do not alter the database
// unless explicitly configured.
if (app.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup"))
{
    const int maxAttempts = 30;
    var migrated = false;

    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            await using var scope = app.Services.CreateAsyncScope();
            var database = scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();
            await database.Database.MigrateAsync();
            migrated = true;
            break;
        }
        catch (Exception) when (attempt < maxAttempts)
        {
            app.Logger.LogWarning(
                "Database is not ready yet (attempt {Attempt}/{MaxAttempts}); retrying in 2 seconds.",
                attempt,
                maxAttempts);
            await Task.Delay(TimeSpan.FromSeconds(2));
        }
    }

    if (!migrated)
    {
        throw new InvalidOperationException(
            "The RaceDay database migrations could not be applied.");
    }
}

// Configure HTTP pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Configuration.GetValue<bool>("Docker:DisableHttpsRedirection"))
{
    app.UseHttpsRedirection();
}

// Authentication must come before authorization.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
