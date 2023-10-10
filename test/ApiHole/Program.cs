using ApiHole.Formatters;
using UkrGuru.SqlJson.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSqlJson(builder.Configuration.GetConnectionString("DefaultConnection"))
    .AddSqlJsonExt(logLevel: builder.Configuration.GetValue<DbLogLevel>("Logging:LogLevel:UkrGuru.SqlJson"),
        initDb: builder.Configuration.GetValue<bool>("AppSettings:InitDb"));

builder.Services.AddControllers(options =>
{
    options.InputFormatters.Insert(0, new PlaintextMediaTypeFormatter());
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
