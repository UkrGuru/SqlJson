# UkrGuru.SqlJson: A Library for Simplifying Data Access in .NET

## Introduction

If you are a .NET developer who works with SQL Server databases, you may have encountered some challenges when it comes to writing queries, passing parameters, and processing results. For example, you may have to deal with complex data types, such as arrays, objects, or JSON, that are not natively supported by SQL Server. You may also have to write a lot of boilerplate code to handle data conversions, validations, and exceptions. And you may have to use different methods or libraries depending on whether you want to execute a dynamic query, a stored procedure, or an asynchronous operation.

Wouldn't it be nice if there was a library that could simplify all these tasks and make data access in .NET more efficient and enjoyable? Well, there is! It's called UkrGuru.SqlJson and it's a library that simplifies the interaction between .NET applications and SQL Server databases. It allows developers to automatically normalize input parameters and deserialize the result. It also supports dynamic queries, stored procedures, and asynchronous operations.

In this article, we will introduce the features and benefits of UkrGuru.SqlJson and show you how to use it in your ASP.NET Core projects.

## Features and Benefits

UkrGuru.SqlJson is a library that aims to make data access in .NET easier and faster. Here are some of the features and benefits that it offers:

- Automatic parameter normalization: You can use any query or procedure that has only one @Data parameter, or no parameter at all. UkrGuru.SqlJson will auto serialize the complex parameter to JSON format. This means you don't have to worry about converting your data types or formatting your input values.
- Automatic result deserialization: You can format the result as a single column SELECT statement. UkrGuru.SqlJson will auto deserialize the result to the desired type. This means you don't have to parse the JSON string or map the columns manually.
- Dynamic queries: You can execute any SQL query using the `ExecuteAsync` method. UkrGuru.SqlJson will handle the connection management, command execution, and result processing for you.
- Stored procedures: You can execute any stored procedure using the `ReadAsync`, `CreateAsync`, `UpdateAsync`, or `DeleteAsync` methods. UkrGuru.SqlJson will handle the parameter passing, command execution, and result processing for you.
- Asynchronous operations: You can use the `await` keyword to perform asynchronous data access operations. UkrGuru.SqlJson will handle the task creation, cancellation, and exception handling for you.

## Installation

To use UkrGuru.SqlJson library in your ASP.NET Core project, you need to follow these steps:

1. Install the UkrGuru.SqlJson package from NuGet?.
2. Open the AppSettings.json files and add the "UkrGuru.SqlJson" and "DefaultConnection" elements.

json
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


3. Open the Program.cs file and register the UkrGuru SqlJson services and extensions:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSqlJson(builder.Configuration.GetConnectionString("DefaultConnection"));

// More other services here ...

var app = builder.Build();
```

## Usage

To use UkrGuru.SqlJson library in your code, you need to inject an instance of `IDbService` into your class constructor. For example:

```csharp
public class ProductsService
{
    private readonly IDbService db;

    public ProductsService(IDbService db)
    {
        this.db = db;
    }
}
```

Then, you can use the methods of `IDbService` to perform data access operations. For example:

```csharp
// Get a list of products
public async Task<List<ProductDto>> GetProductsAsync()
{
    return await db.ReadAsync<List<ProductDto>>("Products_Grd");
}

// Get a product by id
public async Task<ProductDto> GetProductByIdAsync(int id)
{
    return await db.ReadAsync<ProductDto>("Products_Get", id);
}

// Insert a new product
public async Task<int?> InsertProductAsync(ProductDto product)
{
    return await db.CreateAsync<int?>("Products_Ins", product);
}

// Update an existing product
public async Task UpdateProductAsync(ProductDto product)
{
    await db.UpdateAsync("Products_Upd", product);
}

// Delete a product by id
public async Task DeleteProductByIdAsync(int id)
{
    await db.DeleteAsync("Products_Del", id);
}
```

Note that the methods of `IDbService` expect the name of the query or procedure and the input parameter as arguments. The input parameter can be any .NET type, such as a primitive, an object, or a collection. UkrGuru.SqlJson will auto serialize the parameter to JSON format and pass it to the query or procedure as a single @Data parameter.

The query or procedure should return a single column SELECT statement that contains the result in JSON format. UkrGuru.SqlJson will auto deserialize the result to the desired type and return it to the caller.

For example, here are the definitions of the stored procedures used in the code above:

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
SELECT *
FROM OPENJSON(@Data) WITH (
    ProductName varchar(50),
    CategoryName varchar(20),
    QuantityPerUnit varchar(20),
    UnitPrice smallmoney,
    UnitsInStock int,
    UnitsOnOrder int,
    ReorderLevel int,
    Discontinued bit
)
SELECT SCOPE_IDENTITY()
GO

CREATE OR ALTER PROCEDURE [Products_Upd]
@Data nvarchar(500)
AS
UPDATE Products
SET ProductName = D.ProductName,
    CategoryName = D.CategoryName,
    QuantityPerUnit = D.QuantityPerUnit,
    UnitPrice = D.UnitPrice,
    UnitsInStock = D.UnitsInStock,
    UnitsOnOrder = D.UnitsOnOrder,
    ReorderLevel = D.ReorderLevel,
    Discontinued = D.Discontinued
FROM OPENJSON(@Data) WITH (
    ProductId int,
    ProductName varchar(50),
    CategoryName varchar(20),
    QuantityPerUnit varchar(20),
    UnitPrice smallmoney,
    UnitsInStock int,
    UnitsOnOrder int,
    ReorderLevel int,
    Discontinued bit
) D
WHERE Products.ProductId = D.ProductId
GO
```

As you can see, UkrGuru.SqlJson allows you to write simple and concise code to access SQL Server data. You don't have to deal with complex data types, data conversions, or data mappings. You just have to write your