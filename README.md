# UkrGuru.SqlJson
[![Nuget](https://img.shields.io/nuget/v/UkrGuru.SqlJson)](https://www.nuget.org/packages/UkrGuru.SqlJson/)
[![Donate](https://img.shields.io/badge/Donate-PayPal-yellow.svg)](https://www.paypal.com/donate/?hosted_button_id=BPUF3H86X96YN)

The UkrGuru.SqlJson package provides easy and fast database connectivity to SQL Server for .NET applications, and EF Core is not involved in this.

## Background
I'm an old software developer with over 20 years of experience and have written a lot of unique code in my life. In my practice of working with data, I usually use SQL Server and stored procedures to execute queries of any complexity. Last year I tried to use the Microsoft EF Core framework, but I always lacked the power that I had when using the procedures earlier. Eventually my patience ran out and I created a minimally simple UkrGuru.SqlJson package for modern data processing, and now I want to share this knowledge with you...
## Installation

### 1. Add a new DefaultConnection to your database in appsettings.json
```json
"ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BlazorAppDemo;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

### 2. Open the ~/Program.cs file and register the UkrGuru SqlJson service:
```c#
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSqlJson(builder.Configuration.GetConnectionString("DefaultConnection"));

// More other services here ... 

var app = builder.Build();
```

## Samples of code

### DbHelper: how to use?
```c#
@code {
    public List<ProductDto> Products { get; set; } = new();

    protected override async Task InitData() { Title ??= "Products"; await Task.CompletedTask; }

    protected override async Task LoadData() { 
        Products = await DbHelper.ExecAsync<List<ProductDto>>("Products_Grd") ?? new(); 
    }

    protected override async Task InsItemAsync(GridCommandEventArgs args) 
        => await DbHelper.ExecAsync<ProductDto>("Products_Ins", (ProductDto)args.Item);

    protected override async Task UpdItemAsync(GridCommandEventArgs args) 
        => await DbHelper.ExecAsync("Products_Upd", (ProductDto)args.Item);

    protected override async Task DelItemAsync(GridCommandEventArgs args) 
        => await DbHelper.ExecAsync("Products_Del", ((ProductDto)args.Item)?.ProductId);
}
```

### Crud.DbService: how to use?
```c#
public class HttpComponent : ComponentBase
{
    [Inject]
    private UkrGuru.SqlJson.Crud.IDbService CrudDb { get; set; }

    public async Task<T?> CreateAsync<T>(string cmdText, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    => await CrudDb.CreateAsync<T?>(cmdText, data, timeout, cancellationToken);

    public async Task<T?> ReadAsync<T>(string cmdText, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
        => await CrudDb.ReadAsync<T?>(cmdText, data, timeout, cancellationToken);

    public async Task UpdateAsync(string cmdText, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
        => await CrudDb.UpdateAsync(cmdText, data, timeout, cancellationToken);

    public async Task DeleteAsync(string cmdText, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
        => await CrudDb.DeleteAsync(cmdText, data, timeout, cancellationToken);
}
```

### Crud.ApiDbService: how to use?
```c#
[ApiController]
[Route("[controller]")]
public class ApiCrudController : ControllerBase
{
    public ApiCrudController(Crud.IDbService db) => _db = db;

    private readonly Crud.IDbService _db;

    private readonly string _suffix = "_api";

    [HttpPost("{proc}")]
    public async Task<string?> Create(string proc, [FromBody] object? data = null)
    {
        try
        {
            return await _db.CreateAsync<string?>($"{proc}{_suffix}", data);
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}. Proc={proc}";
        }
    }

    [HttpGet("{proc}")]
    public async Task<string?> Read(string proc, string? data = null)
    {
        try
        {
            return await _db.ReadAsync<string?>($"{proc}{_suffix}", data);
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}. Proc={proc}";
        }
    }


    [HttpPut("{proc}")]
    public async Task<string?> Update(string proc, [FromBody] object? data = null)
    {
        try
        {
            await _db.UpdateAsync($"{proc}{_suffix}", data);
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}. Proc={proc}";
        }
        return null;
    }

    [HttpDelete("{proc}")]
    public async Task<string?> Delete(string proc, string? data = null)
    {
        try
        {
            await _db.DeleteAsync($"{proc}{_suffix}", data);
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}. Proc={proc}";
        }
        return null;
    }
}
```

## Standard for procedures

UkrGuru.SqlJson automatically normalizes input parameters and deserializes the result.

So you must follow the next requirements:
1. If cmdText.Lenght is at less 50, then command.CommandType will be set to CommandType.StoredProcedure, otherwise command.CommandType will still be CommandType.Text.
2. You can use procedures with a single @Data parameter of any type, or no parameter.
3. It is required to prepare the result in json format with the option "FOR JSON PATH" for List or "FOR JSON PATH, WITHOUT_ARRAY_WRAPPER" for one record.


```sql
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [Products_Del]
    @Data int
AS
DELETE Products
WHERE (ProductId = @Data)
GO

CREATE OR ALTER PROCEDURE [Products_Get]
    @Data int
AS
SELECT ProductId, ProductName, CategoryName, QuantityPerUnit, UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued
FROM Products
WHERE (ProductId = @Data)
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
GO

CREATE OR ALTER PROCEDURE [Products_Grd]
AS
SELECT ProductId, ProductName, CategoryName, QuantityPerUnit, UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued
FROM Products
FOR JSON PATH
GO

CREATE OR ALTER PROCEDURE [Products_Ins]
	@Data nvarchar(500)  
AS
INSERT INTO Products (ProductName, CategoryName, QuantityPerUnit, UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued)
SELECT ProductName, CategoryName, QuantityPerUnit, UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued
FROM OPENJSON(@Data) 
WITH (ProductName varchar(50), CategoryName varchar(20), QuantityPerUnit varchar(20), 
	UnitPrice smallmoney, UnitsInStock int, UnitsOnOrder int, ReorderLevel int, Discontinued bit
)
GO

CREATE OR ALTER PROCEDURE [Products_Upd]
	@Data nvarchar(500)  
AS
UPDATE P
SET P.ProductName = D.ProductName, P.CategoryName = D.CategoryName, P.QuantityPerUnit = D.QuantityPerUnit,
	P.UnitPrice = D.UnitPrice, P.UnitsInStock = D.UnitsInStock, P.UnitsOnOrder = D.UnitsOnOrder,
	P.ReorderLevel = D.ReorderLevel, P.Discontinued = D.Discontinued
FROM Products P
CROSS JOIN (SELECT * FROM OPENJSON(@Data) 
    WITH (ProductName varchar(50), CategoryName varchar(20), QuantityPerUnit varchar(20), 
	UnitPrice smallmoney, UnitsInStock int, UnitsOnOrder int, ReorderLevel int, Discontinued bit)) D
WHERE P.ProductId = JSON_VALUE(@Data,'$.ProductId')
GO


```
