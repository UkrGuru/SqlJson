# UkrGuru.SqlJson
[![Nuget](https://img.shields.io/nuget/v/UkrGuru.SqlJson)](https://www.nuget.org/packages/UkrGuru.SqlJson/)
[![Donate](https://img.shields.io/badge/Donate-PayPal-yellow.svg)](https://www.paypal.com/donate/?hosted_button_id=BPUF3H86X96YN)

UkrGuru.SqlJson is a library that simplifies the interaction between .NET applications and SQL Server databases. It allows developers to use JSON as a common data format for both the application and the database layers, eliminating the need for complex mapping or serialization. UkrGuru.SqlJson also supports dynamic queries, stored procedures and asynchronous operations. With UkrGuru.SqlJson, you can access SQL Server data with minimal code and maximum performance.

## Installation

To use UkrGuru SqlJson library in your ASP.NET Core project, you need to follow these steps:

### 1. Install the UkrGuru.SqlJson package from NuGet.

### 2. Open the AppSettings.json files and add the "UkrGuru.SqlJson" and "DefaultConnection" elements.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BlazorAppDemo;Trusted_Connection=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### 2. Open the Program.cs file and register the UkrGuru SqlJson services and extensions:

```c#
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSqlJson(builder.Configuration.GetConnectionString("DefaultConnection"));

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
2. OUTPUT: Should prepare the result in one column SELECT sqltype or json types. See more examples bellow ...


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

SELECT SCOPE_IDENTITY()
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
