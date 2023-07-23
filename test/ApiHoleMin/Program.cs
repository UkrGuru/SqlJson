using Microsoft.AspNetCore.Mvc;
using UkrGuru.SqlJson;
using UkrGuru.SqlJson.Extensions;

const string ApiHolePattern = "ApiHole";
const string ApiProcSufix = "_Api";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSqlJson(builder.Configuration.GetConnectionString("DefaultConnection"))
    .AddSqlJsonExt(logLevel: builder.Configuration.GetValue<DbLogLevel>("Logging:LogLevel:UkrGuru.SqlJson"),
        initDb: builder.Configuration.GetValue<bool>("AppSettings:InitDb"));

var app = builder.Build();

app.UseHttpsRedirection();

app.MapPost($"{ApiHolePattern}/{{proc}}", async (IDbService db, string proc, [FromBody] object? data) =>
{
    try
    {
        return await db.CreateAsync<string?>($"{proc}{ApiProcSufix}", Convert.ToString(data)) ?? string.Empty;
    }
    catch (Exception ex)
    {
        return $"Error: {ex.Message}. Proc={proc}";
    }
});

app.MapGet($"{ApiHolePattern}/{{proc}}", async (IDbService db, string proc, string? data) =>
{
    try
    {
        return await db.ReadAsync<string?>($"{proc}{ApiProcSufix}", data) ?? string.Empty;
    }
    catch (Exception ex)
    {
        return $"Error: {ex.Message}. Proc={proc}";
    }
});

app.MapPut($"{ApiHolePattern}/{{proc}}", async (IDbService db, string proc, [FromBody] object? data) =>
{
    try
    {
        await db.UpdateAsync($"{proc}{ApiProcSufix}", Convert.ToString(data));
        return string.Empty;
    }
    catch (Exception ex)
    {
        return $"Error: {ex.Message}. Proc={proc}";
    }
});

app.MapDelete($"{ApiHolePattern}/{{proc}}", async (IDbService db, string proc, string? data) =>
{
    try
    {
        await db.DeleteAsync($"{proc}{ApiProcSufix}", data);
        return string.Empty;
    }
    catch (Exception ex)
    {
        return $"Error: {ex.Message}. Proc={proc}";
    }
});

app.Run();