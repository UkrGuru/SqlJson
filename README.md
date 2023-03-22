# UkrGuru.SqlJson
[![Nuget](https://img.shields.io/nuget/v/UkrGuru.SqlJson)](https://www.nuget.org/packages/UkrGuru.SqlJson/)
[![Donate](https://img.shields.io/badge/Donate-PayPal-yellow.svg)](https://www.paypal.com/donate/?hosted_button_id=BPUF3H86X96YN)

UkrGuru.SqlJson is a package for modern data processing that can be used to execute queries of any complexity. It is designed to be easy and fast to use, and it automatically normalizes input parameters and deserializes the result.

Some of the advantages of UkrGuru.SqlJson include:
*	It is easy to use and fast to execute queries of any complexity.
*	It is designed to be used with SQL Server and stored procedures.
*	It automatically normalizes input parameters and deserializes the result.
*	It is a minimally simple package for modern data processing.

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
