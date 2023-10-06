using ApiHole.Formatters;
using UkrGuru.SqlJson.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSqlJson(builder.Configuration.GetConnectionString("DefaultConnection"))
    .AddSqlJsonExt(logLevel: builder.Configuration.GetValue<DbLogLevel>("Logging:LogLevel:UkrGuru.SqlJson"),
        initDb: builder.Configuration.GetValue<bool>("AppSettings:InitDb"));

builder.Services.AddControllers(options =>
{
    options.InputFormatters.Insert(0, new PlaintextMediaTypeFormatter());
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
