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
    public const string DbName = "SqlJsonTest5";
    public const string ConnectionString = $"Server=(localdb)\\mssqllocaldb;Database={DbName};Trusted_Connection=True";

    public static bool DbOk { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public static IConfiguration Configuration { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public static byte[] TestBytes1k = new byte[1024];
    public static string TestString1k => Convert.ToBase64String(TestBytes1k);
    public static char[] TestChars1k => TestString1k.ToCharArray();

    public static byte[] TestBytes5k = new byte[1024 * 5];
    public static string TestString5k => Convert.ToBase64String(TestBytes5k);
    public static char[] TestChars5k => TestString5k.ToCharArray();

    public static byte[] TestBytes55k = new byte[1024 * 55];
    public static string TestString55k => Convert.ToBase64String(TestBytes55k);
    public static char[] TestChars55k => TestString55k.ToCharArray();

    public static Random Random = new Random(2511);

    //public static byte[] TestBytes55k_Hash256 { get; set; }
    //public static byte[] TestString55k_Hash256 { get; set; }
    //public static byte[] TestChars55k_Hash256 { get; set; }

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
            { "ConnectionStrings:DefaultConnection", connectionString },
            { "Logging:LogLevel:UkrGuru.SqlJson", "Information" }
        };

        Configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        Random.NextBytes(TestBytes1k);
        Random.NextBytes(TestBytes5k);
        Random.NextBytes(TestBytes55k);

        //using (var sha256 = SHA256.Create())
        //{
        //    TestBytes55k_Hash256 = sha256.ComputeHash(TestBytes55k);
        //    TestChars55k_Hash256 = TestString55k_Hash256 = sha256.ComputeHash(Convert.FromBase64String(TestString55k));
        //}

        DbOk = true;
    }

    [Fact]
    public void CanInitDbs() { }
}