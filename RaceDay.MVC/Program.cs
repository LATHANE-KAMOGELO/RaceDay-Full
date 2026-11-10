
using RaceDay.MVC.Services;

var builder = WebApplication.CreateBuilder(args);

// Register MVC services.
builder.Services.AddControllersWithViews(options =>
    options.Filters.Add(
        new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute()));

// Register API service with the API base address.
builder.Services.AddHttpClient<ApiService>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["ApiSettings:BaseUrl"]!);
});

// Configure server-side session.
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
