// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Configuration;
using System.Reflection;
using UkrGuru.SqlJson.Extensions;

namespace UkrGuru.SqlJson;

public class GlobalTests
{
    public const string DbName = "SqlJsonTest";

    private static string? _connectionString;

    public static string ConnectionString
    {
        get
        {
            _connectionString ??= $"Server=(localdb)\\mssqllocaldb;Database={DbName};Trusted_Connection=True";

            return _connectionString;
        }
    }

    private static IConfiguration _configuration;

    public static IConfiguration Configuration
    {
        get
        {
            if (_configuration == null)
            {
                var inMemorySettings = new Dictionary<string, string?>() {
                    { "ConnectionStrings:DefaultConnection", ConnectionString},
                    { "Logging:LogLevel:UkrGuru.SqlJson", "Information" }
                };

                _configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(inMemorySettings)
                    .Build();
            }

            return _configuration;
        }
    }

    private static HttpClient _http;

    public static HttpClient Http
    {
        get
        {
            _http ??= new HttpClient() { BaseAddress = new Uri("https://localhost:7271/") };

            return _http;
        }
    }

    public static Random Random = new Random(2511);

    private static byte[]? _testBytes1k, _testBytes5k, _testBytes55k;

    public static byte[] TestBytes1k
    {
        get
        {
            if (_testBytes1k == null)
            {
                _testBytes1k = new byte[1024];
                Random.NextBytes(_testBytes1k);
            }

            return _testBytes1k;
        }
    }
    public static string TestString1k => Convert.ToBase64String(TestBytes1k);
    public static char[] TestChars1k => TestString1k.ToCharArray();

    public static byte[] TestBytes5k
    {
        get
        {
            if (_testBytes5k == null)
            {
                _testBytes5k = new byte[1024 * 5];
                Random.NextBytes(_testBytes5k);
            }

            return _testBytes5k;
        }
    }
    public static string TestString5k => Convert.ToBase64String(TestBytes5k);
    public static char[] TestChars5k => TestString5k.ToCharArray();

    public static byte[] TestBytes55k
    {
        get
        {
            if (_testBytes55k == null)
            {
                _testBytes55k = new byte[1024 * 55];
                Random.NextBytes(_testBytes55k);
            }

            return _testBytes55k;
        }
    }
    public static string TestString55k => Convert.ToBase64String(TestBytes55k);
    public static char[] TestChars55k => TestString55k.ToCharArray();

    public class Region
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    public enum UserType
    {
        Guest,
        User,
        Manager,
        Admin,
        SysAdmin
    }

    public GlobalTests()
    {
        //var connectionString = $"Server=(localdb)\\mssqllocaldb;Database={DbName};Trusted_Connection=True";

        //DbHelper.ConnectionString = connectionString.Replace(DbName, "master");

        //DbHelper.Exec($"IF DB_ID('{DbName}') IS NULL CREATE DATABASE {DbName};");

        //DbHelper.ConnectionString = connectionString;

        //Assembly.GetAssembly(typeof(DbHelper)).InitDb();

        //Assembly.GetExecutingAssembly().InitDb();
    }

    [Fact]
    public void CanInitDbs()
    {
        DbHelper.ConnectionString = ConnectionString;

        Assembly.GetAssembly(typeof(DbHelper)).InitDb();

        Assembly.GetExecutingAssembly().InitDb();
    }
}