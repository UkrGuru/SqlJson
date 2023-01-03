using BlazorAppDemo.Data;
using System.Reflection;
using UkrGuru.Extensions;
using UkrGuru.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSqlJson(builder.Configuration.GetConnectionString("DefaultConnection"))
    .AddSqlJsonExt(builder.Configuration.GetValue<DbLogLevel>("Logging:LogLevel:UkrGuru.SqlJson"));

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddTelerikBlazor();

var app = builder.Build();

// Create tables and stored procedures for Demo database.
Assembly.GetExecutingAssembly().InitDb();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
