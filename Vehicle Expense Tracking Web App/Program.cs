using VehicleExpenseTrackingWebApp.Components;
using VehicleExpenseTrackingWebApp.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseBaglantisi")));

// Add YakitTuketimiService
builder.Services.AddScoped<VehicleExpenseTrackingWebApp.Services.YakitTuketimiService>();

// Add AracSearchService
builder.Services.AddScoped<VehicleExpenseTrackingWebApp.Services.AracSearchService>();

// Add RaporIstatistikService
builder.Services.AddScoped<VehicleExpenseTrackingWebApp.Services.RaporIstatistikService>();

// Add KarsilastirmaService
builder.Services.AddScoped<VehicleExpenseTrackingWebApp.Services.KarsilastirmaService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
