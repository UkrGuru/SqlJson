# UkrGuru.SqlJson
[![Nuget](https://img.shields.io/nuget/v/UkrGuru.SqlJson)](https://www.nuget.org/packages/UkrGuru.SqlJson/)
[![Donate](https://img.shields.io/badge/Donate-PayPal-yellow.svg)](https://www.paypal.com/donate/?hosted_button_id=BPUF3H86X96YN)

UkrGuru.SqlJson is very easy and fast access to the Sql Server database and EF Core is not involved in this.

## Background
I'm an old software developer with over 20 years of experience and have written a lot of unique code in my life. In my practice of working with data, I usually use SQL Server and stored procedures to execute queries of any complexity. Last year I tried to use the Microsoft EF Core framework, but I always lacked the power that I had when using the procedures earlier. Eventually my patience ran out and I created a minimally simple UkrGuru.SqlJson package for modern data processing, and now I want to share this knowledge with you...
## Installation

### 1. Add "DefaultConnection" and "UkrGuru.SqlJson" elements in AppSettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BlazorAppDemo;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "UkrGuru.SqlJson": "Debug"
    }
  },
  "AllowedHosts": "*"
}
```

### 2. Open ~/Program.cs file and register UkrGuru SqlJson services and extensions:
```c#
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSqlJson(builder.Configuration.GetConnectionString("DefaultConnection"))
    .AddSqlJsonExt(builder.Configuration.GetValue<DbLogLevel>("Logging:LogLevel:UkrGuru.SqlJson"));

// More other services here ... 

var app = builder.Build();
```

## Samples of code

```c#
@code {
    public List<ProductDto>? Products { get; set; }

    protected override async Task InitData() { Title ??= "Products"; await Task.CompletedTask; }

    protected override async Task LoadData() {
        Products = await db.ReadAsync<List<ProductDto>>("Products_Grd");
    }

    protected override async Task InsItemAsync(GridCommandEventArgs args) 
        => await db.CreateAsync<int?>("Products_Ins", (ProductDto)args.Item);

    protected override async Task UpdItemAsync(GridCommandEventArgs args) 
        => await db.UpdateAsync("Products_Upd", (ProductDto)args.Item);

    protected override async Task DelItemAsync(GridCommandEventArgs args) 
        => await db.DeleteAsync("Products_Del", ((ProductDto)args.Item)?.ProductId);
}
```

## Standard for procedures

UkrGuru.SqlJson automatically normalizes input parameters and deserializes the result.

Requirements:
1. INPUT: Must use a query or procedure with only one @Data parameter of any type, or no parameter.
2. OUTPUT: Should prepare the result in one column SELECT varchar or json types. See more examples bellow ...


```sql
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [Products_Del]
    @Data int
AS
DELETE Products
WHERE ProductId = @Data
GO

CREATE OR ALTER PROCEDURE [Products_Get]
    @Data int
AS
SELECT *
FROM Products
WHERE ProductId = @Data
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
GO

CREATE OR ALTER PROCEDURE [Products_Grd]
AS
SELECT *
FROM Products
FOR JSON PATH
GO

CREATE OR ALTER PROCEDURE [Products_Ins]
	@Data nvarchar(500)  
AS
INSERT INTO Products 
SELECT * FROM OPENJSON(@Data) 
WITH (ProductName varchar(50), CategoryName varchar(20), QuantityPerUnit varchar(20), 
	UnitPrice smallmoney, UnitsInStock int, UnitsOnOrder int, ReorderLevel int, Discontinued bit)

SELECT CAST(SCOPE_IDENTITY() AS varchar(10))
GO

CREATE OR ALTER PROCEDURE [Products_Upd]
	@Data nvarchar(500)  
AS
UPDATE Products
SET ProductName = D.ProductName, CategoryName = D.CategoryName, QuantityPerUnit = D.QuantityPerUnit,
	UnitPrice = D.UnitPrice, UnitsInStock = D.UnitsInStock, UnitsOnOrder = D.UnitsOnOrder,
	ReorderLevel = D.ReorderLevel, Discontinued = D.Discontinued
FROM OPENJSON(@Data) 
WITH (ProductId int, ProductName varchar(50), CategoryName varchar(20), QuantityPerUnit varchar(20), 
	UnitPrice smallmoney, UnitsInStock int, UnitsOnOrder int, ReorderLevel int, Discontinued bit) D
WHERE Products.ProductId = D.ProductId
GO
```
