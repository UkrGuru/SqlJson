using ApiHole.Formatters;
using System.Reflection;
using UkrGuru.SqlJson;
using UkrGuru.SqlJson.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSqlJson(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddSqlJsonExt(builder.Configuration.GetValue<DbLogLevel>("Logging:LogLevel:UkrGuru.SqlJson"));

builder.Services.AddControllersWithViews(options =>
{
    options.InputFormatters.Insert(0, new PlaintextMediaTypeFormatter());
});
builder.Services.AddRazorPages();

var app = builder.Build();

Assembly.GetExecutingAssembly().InitDb();

typeof(ApiDbService).Assembly.InitDb();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
