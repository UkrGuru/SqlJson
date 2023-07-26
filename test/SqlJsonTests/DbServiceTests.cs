// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace UkrGuru.SqlJson;

public class DbServiceTests
{
    private readonly IConfiguration _configuration;

    private readonly IDbService _db;

    public DbServiceTests()
    {
        int i = 0; while (!GlobalTests.DbOk && i++ < 100) { Thread.Sleep(100); }

        var inMemorySettings = new Dictionary<string, string?>() {
            { "ConnectionStrings:DefaultConnection", GlobalTests.ConnectionString},
            { "Logging:LogLevel:UkrGuru.SqlJson", "Information" }
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _db = new DbService(_configuration);
    }

    public static IEnumerable<object[]> GetData4CanNormalize(int numTests)
    {
        var guid = Guid.Empty;
        string data = JsonSerializer.Serialize(new { Name = "Proc1" })!;
        var allData = new List<object[]>
        {
            new object[] { "str1", "str1" },
            new object[] { true, "True"  },
            new object[] { false, "False" },
            new object[] { (byte)1, "1" },
            //new object[] { new byte[] { 1, 2 }, new byte[] { 1, 2 } },
            //new object[] { new char[] { '1', '2' }, new char[] { '1', '2' } },
            new object[] { 123.45d, "123.45"  },
            new object[] { 123.45f, "123.45"  },
            new object[] { 123, "123"  },
            new object[] { 12345, "12345"  },
            new object[] { 1234567890, "1234567890" },
            //new object[] { new DateOnly(2000, 1, 1), "" },
            new object[] { new DateTime(2000, 1, 1, 1, 1, 1), "01/01/2000 01:01:01" },
            //new object[] { new DateTimeOffset(new DateTime(2000, 1, 1, 1, 1, 1)), "" },
            //new object[] { new TimeOnly(1, 1, 1), "" },
            //new object[] { guid, "00000000-0000-0000-0000-000000000000" },
            //new object[] { new { Name = "Proc1" }, data },
            //new object[] { JsonSerializer.Deserialize<dynamic?>(data)!, data }
        };

        return allData.Take(numTests);
    }

    [Theory]
    [MemberData(nameof(GetData4CanNormalize), parameters: 17)]
    public async Task CanExecAsync(string? data, object expected) => Assert.Equal(expected, await _db.ExecAsync<string?>("ProcVar", data));

    [Fact]
    public async Task CanHaveResultsAsync()
    {
        //Assert.Null(await _db.ExecAsync<int?>("ProcNull"));

        //Assert.Null(await _db.ExecAsync<int?>("ProcVar", null));
        //Assert.Equal(1, await _db.ExecAsync<int?>("ProcVar", 1));

        //Assert.Equal("Data", await _db.ExecAsync<string?>("ProcVar", "Data"));

        //Assert.Equal(true, await _db.ExecAsync<bool?>("ProcVar", true));

        //Assert.Equal(Guid.Empty, await _db.ExecAsync<Guid?>("ProcVar", Guid.Empty));

        //Assert.Equal('X', await _db.ExecAsync<char?>("ProcVar", 'X'));

        //Assert.Equal((byte)1, await _db.ExecAsync<byte?>("ProcVar", (byte)1));
        //Assert.Equal(1, await _db.ExecAsync<int?>("ProcVar", 1));
        //Assert.Equal((long)1, await _db.ExecAsync<long?>("ProcVar", (long)1));
        //Assert.Equal(1.0f, await _db.ExecAsync<float?>("ProcVar", 1.0f));
        //Assert.Equal(1.0d, await _db.ExecAsync<double?>("ProcVar", 1.0d));
        //Assert.Equal(1.0m, await _db.ExecAsync<decimal?>("ProcVar", 1.0m));

        //Assert.Equal(new DateOnly(2000, 1, 1), await _db.ExecAsync<DateOnly?>("ProcVar", new DateOnly(2000, 1, 1)));
        //Assert.Equal(new DateTime(2000, 1, 1, 1, 1, 1), await _db.ExecAsync<DateTime?>("ProcVar", new DateTime(2000, 1, 1, 1, 1, 1)));
        //Assert.Equal(new DateTimeOffset(new DateTime(2000, 1, 1)), await _db.ExecAsync<DateTimeOffset?>("ProcVar", new DateTimeOffset(new DateTime(2000, 1, 1))));
        //Assert.Equal(new TimeOnly(1, 1, 1), await _db.ExecAsync<TimeOnly?>("ProcVar", new TimeOnly(1, 1, 1)));
        //Assert.Equal(new TimeSpan(1, 1, 1), await _db.ExecAsync<TimeSpan?>("ProcVar", new TimeSpan(1, 1, 1)));

        Assert.Equal("John", await _db.ExecAsync<string?>("ProcObj", new { Name = "John" }));
        //Assert.Equal("John", await _db.ExecAsync<string?>("SELECT JSON_VALUE(@Data, '$.Name');", new { Name = "John" }));

        //var rec1 = await _db.ExecAsync<JsonObject>("SELECT 1 Id, 'John' Name FOR JSON PATH, WITHOUT_ARRAY_WRAPPER");
        //Assert.NotNull(rec1);
        //Assert.Equal(1, (int?)rec1["Id"]);
        //Assert.Equal("John", (string?)rec1["Name"]);

        //var recs = await _db.ExecAsync<List<JsonObject>>("SELECT 1 Id, 'John' Name UNION ALL SELECT 2 Id, 'Mike' Name FOR JSON PATH");
        //Assert.NotNull(recs);
        //Assert.Equal(2, recs.Count);
        //Assert.Equal(1, (int?)recs[0]["Id"]);
        //Assert.Equal("John", (string?)recs[0]["Name"]);
        //Assert.Equal(2, (int?)recs[1]["Id"]);
        //Assert.Equal("Mike", (string?)recs[1]["Name"]);
    }

    //[Fact]
    //public void CanExec()
    //{
    //    Assert.Null(_db.Exec<bool?>("SELECT NULL"));
    //    Assert.True(_db.Exec<bool?>("SELECT @Data", true));

    //    Assert.Equal(-1, _db.Exec("ProcNull"));
    //    Assert.Equal(1, _db.Exec<int?>("ProcInt", 1));
    //}

    //[Fact]
    //public async Task CanExecAsync()
    //{
    //    Assert.Null(await _db.ExecAsync<bool?>("SELECT NULL"));
    //    Assert.True(await _db.ExecAsync<bool?>("SELECT @Data", true));

    //    Assert.Equal(-1, await _db.ExecAsync("ProcNull"));
    //    Assert.Equal(1, await _db.ExecAsync<int?>("ProcInt", 1));
    //}
}