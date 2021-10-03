# UkrGuru.SqlJson
#### Alternative easy way to run Sql Server procedures without using Microsoft Entity Framework Core

[![Nuget](https://img.shields.io/nuget/v/UkrGuru.SqlJson)](https://www.nuget.org/packages/UkrGuru.SqlJson/)

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

## License
The UkrGuru.SqlJson package is an open source product licensed under:

***[MIT license](https://raw.githubusercontent.com/UkrGuru/SqlJson/main/LICENSE.txt)**

All source code is **&copy; Oleksandr Viktor (UkrGuru)**, regardless of changes made to them. Any source code modifications must leave the original copyright code headers intact if present.

There's no charge to use, integrate or modify the code for this project. You are free to use it in personal, commercial, government and any other type of application and you are free to modify the code for use in your own projects.

### Donate
If you find this library useful, consider making a small donation. Contact with me by email (ukrguru@gmail.com) for Payoneer Invoice ...
