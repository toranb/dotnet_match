using Match.Components;
using Match.Services;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
