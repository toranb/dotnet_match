using Match.Components;
using Match.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IGameService, GameService>();

builder.Configuration.AddEnvironmentVariables();
var keyPath = builder.Configuration["PROTECTION_DIR"];
var keyString = builder.Configuration["DOTNET_KEYS"];
if (string.IsNullOrEmpty(keyString) || string.IsNullOrEmpty(keyPath))
{
    throw new InvalidOperationException("DOTNET_KEYS environment variable is required.");
}
var protectionPath = Path.Combine(keyPath, keyString);
builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(protectionPath));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Add this after building your app
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();

    // This creates the database if it doesn't exist
    context.Database.EnsureCreated();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
