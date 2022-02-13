# UkrGuru.SqlJson
[![Nuget](https://img.shields.io/nuget/v/UkrGuru.SqlJson)](https://www.nuget.org/packages/UkrGuru.SqlJson/)

This framework is fast and easy access to Sql Server databases only through stored procedures and EF Core is not involved in this.

## Background

I am an old software developer for over 20 years and have written a lot of unique code in my life. In my practice of working with data, I usually use SQL Server and stored procedures to execute queries of any complexity. For the past year, I tried to use the Microsoft EF Core framework, but I always lacked the power that I had when using procedures earlier. In the end, my patience ran out and I created a minimally simple UkrGuru.SqlJson package for modern data manipulation, and now I want to share this knowledge with you...

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

### 3. Open the ~/Startup.cs file and register the UkrGuru SqlJson service:
```c#
public void ConfigureServices(IServiceCollection services)
{
  // more code may be present here
  services.AddSqlJson(Configuration.GetConnectionString("SqlJsonConnection"));
  // more code may be present here
}
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

UkrGuru.SqlJson will automatically serialize C# input parameters list to json and deserialize result in object.

So you must follow the next requirements:
1. You can use procedures without parameters or with 1 specific parameter (@Data varchar)
2. If used FromProcAsync then you need prepare result in json format with "FOR JSON PATH" for List<TEntity> or with "FOR JSON PATH, WITHOUT_ARRAY_WRAPPER" for TEntity


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


## License
The UkrGuru.SqlJson package is an open source product licensed under:

* **[MIT license](https://github.com/UkrGuru/SqlJson/blob/main/LICENSE.txt)**

All source code is **&copy; Oleksandr Viktor (UkrGuru)**, regardless of changes made to them. Any source code modifications must leave the original copyright code headers intact if present.

There's no charge to use, integrate or modify the code for this project. You are free to use it in personal, commercial, government and any other type of application and you are free to modify the code for use in your own projects.

### Donate
If you find this library useful, consider making a small donation.
	Contact with me by email (ukrguru@gmail.com) for Payoneer Invoice ...
