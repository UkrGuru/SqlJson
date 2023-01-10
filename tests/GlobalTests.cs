// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

global using Xunit;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using UkrGuru.Extensions;
using UkrGuru.Extensions.Data;

namespace UkrGuru.SqlJson;

public class GlobalTests
{
    public const string DbName = "SqlJsonTest";

    public static bool DbOk { get; set; }

    public static IConfiguration Configuration { get; set; } 

    public GlobalTests()
    {
        if (DbOk) return;

        var connectionString = $"Server=(localdb)\\mssqllocaldb;Database={DbName};Trusted_Connection=True";

        DbHelper.ConnectionString = connectionString.Replace(DbName, "master");

        DbHelper.Exec($"IF DB_ID('{DbName}') IS NULL CREATE DATABASE {DbName};");

        DbHelper.ConnectionString = connectionString;

        Assembly.GetAssembly(typeof(DbFile)).InitDb();

        Assembly.GetExecutingAssembly().InitDb();

        var inMemorySettings = new Dictionary<string, string?>() {
            {"ConnectionStrings:DefaultConnection", connectionString }
        };

        Configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        DbOk = true;
    }

    [Fact]
    public void CanInitDbs() { }
}