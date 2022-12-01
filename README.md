# UkrGuru.SqlJson
[![Nuget](https://img.shields.io/nuget/v/UkrGuru.SqlJson)](https://www.nuget.org/packages/UkrGuru.SqlJson/)
[![Donate](https://img.shields.io/badge/Donate-PayPal-yellow.svg)](https://www.paypal.com/donate/?hosted_button_id=BPUF3H86X96YN)

This package provides fast and easy access through stored procedures to SQL Server databases, and EF Core is not involved in this.

## Background

I'm an old software developer with over 20 years of experience and have written a lot of unique code in my life. In my practice of working with data, I usually use SQL Server and stored procedures to execute queries of any complexity. Last year I tried to use the Microsoft EF Core framework, but I always lacked the power that I had when using the procedures earlier. Eventually my patience ran out and I created a minimally simple UkrGuru.SqlJson package for modern data processing, and now I want to share this knowledge with you...
## Installation

### 1. Install the nuget package UkrGuru.SqlJson in Visual Studio

#### Package Manager:
```ps
Install-Package UkrGuru.SqlJson
```

#### or the `dotnet` command line:
```cmd
dotnet add package UkrGuru.SqlJson
```

### 2. Add a new SqlJsonConnection to your database in appsettings.json
```json
"ConnectionStrings": {
    "SqlJsonConnection": "Server=localhost;Database=SqlJsonDemo;Integrated Security=SSPI"
}
```

### 3. Open the ~/Program.cs file and register the UkrGuru SqlJson service:
```c#
builder.Services.AddUkrGuruSqlJson(builder.Configuration.GetConnectionString("SqlJsonConnection"));  
```

## Samples of code

### DbService: how to use?
```c#
[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly DbService _db;
    public ContactController(DbService db) => _db = db;

    [HttpGet]
    public async Task<List<Contact>> Get() => await _db.FromProcAsync<List<Contact>>("Contacts_List");

    [HttpGet("{id}")]
    public async Task<Contact> Get(int id) => await _db.FromProcAsync<Contact>("Contacts_Item", id);

    [HttpPost]
    public async Task<Contact> Post([FromBody] Contact item) => await _db.FromProcAsync<Contact>("Contacts_Ins", item);

    [HttpPut("{id}")]
    public async Task Put(int id, [FromBody] Contact item) => await _db.ExecProcAsync("Contacts_Upd", item);

    [HttpDelete("{id}")]
    public async Task Delete(int id) => await _db.ExecProcAsync("Contacts_Del", id);
}
```
### DbHelper: how to use?
```c#
[HttpPost("Post")]
public async Task<Contact> Post([FromBody] Contact item)
{
    return await DbHelper.FromProcAsync<Contact>("Contacts_Ins", item);
}
```

## Standard for procedures

UkrGuru.SqlJson will automatically normalize the input parameter and deserialize the result.

So you must follow the next requirements:
1. You can use procedures without or with one specific @Data parameter only
2. To use FromProcAsync, you need to prepare the result in json format with the "FOR JSON PATH" option for List or "FOR JSON PATH, WITHOUT_ARRAY_WRAPPER" for TEntity,

```sql
CREATE PROCEDURE [dbo].[Contacts_List] 
AS
SELECT Id, FullName, Email, Notes
FROM Contacts
FOR JSON PATH
```

```sql
ALTER PROCEDURE [dbo].[Contacts_Ins]
	@Data nvarchar(max) 
AS
INSERT INTO Contacts (FullName, Email, Notes)
SELECT * FROM OPENJSON(@Data) 
WITH (FullName nvarchar(50), Email nvarchar(100), Notes nvarchar(max))

DECLARE @Id int = SCOPE_IDENTITY()

SELECT Id, FullName, Email, Notes
FROM Contacts
WHERE Id = @Id
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
```
