// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

global using Xunit;
using System.Reflection;
using UkrGuru.Extensions;
using UkrGuru.Extensions.Data;

namespace UkrGuru.SqlJson;

public class GlobalTests
{
    public const string DbName = "SqlJsonTest";

    public const string ConnectionString = $"Data Source=(localdb)\\mssqllocaldb;Database=master;Integrated Security=True;Connect Timeout=30;ConnectRetryCount=0";

    public static bool DbOk { get; set; }

    public static string CommandTest = "SELECT 1;";

    public GlobalTests()
    {
        if (DbOk) return;

        var connectionString = $"Server=(localdb)\\mssqllocaldb;Database={DbName};Trusted_Connection=True";

        DbHelper.ConnectionString = connectionString.Replace(DbName, "master");

        DbHelper.Exec($"IF DB_ID('{DbName}') IS NULL CREATE DATABASE {DbName};");

        DbHelper.ConnectionString = GlobalTests.ConnectionString;

        Assembly.GetAssembly(typeof(DbFile)).InitDb();

        Assembly.GetExecutingAssembly().InitDb();

        DbOk = true;
    }

    [Fact]
    public void CanInitDbs() { }
}