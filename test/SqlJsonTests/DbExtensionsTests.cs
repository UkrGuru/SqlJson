// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;
using System.Text.Json;
using static UkrGuru.SqlJson.GlobalTests;

namespace UkrGuru.SqlJson;

public class DbExtensionsTests
{
    [Theory]
    [InlineData(null, false)]
    [InlineData(typeof(Boolean), false)]
    [InlineData(typeof(Byte), false)]
    [InlineData(typeof(Int16), false)]
    [InlineData(typeof(Int32), false)]
    [InlineData(typeof(Int64), false)]
    [InlineData(typeof(Single), false)]
    [InlineData(typeof(Double), false)]
    [InlineData(typeof(Decimal), false)]
    [InlineData(typeof(DateOnly), false)]
    [InlineData(typeof(DateTime), false)]
    [InlineData(typeof(DateTimeOffset), false)]
    [InlineData(typeof(TimeOnly), false)]
    [InlineData(typeof(TimeSpan), false)]
    [InlineData(typeof(Guid), false)]
    [InlineData(typeof(Char), false)]
    [InlineData(typeof(UserType), false)]
    [InlineData(typeof(Byte[]), true)]
    [InlineData(typeof(Char[]), true)]
    public void IsLongTests(Type? t, bool expected)
        => Assert.Equal(expected, t.IsLong());

    [Theory]
    [InlineData("SELECT 1;", null)]
    [InlineData("SELECT 1;", 1)]
    [InlineData("SELECT 1;", null, 15)]
    [InlineData("SELECT 1;", null, 45)]
    [InlineData("ProcTest", null, null, CommandType.StoredProcedure)]
    public static void CanCreateSqlCommand(string cmdText, object? data = null, int? timeout = null, CommandType expected = CommandType.Text)
    {
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();

        var command = connection.CreateSqlCommand(cmdText, data, timeout);

        Assert.NotNull(command);
        Assert.Equal(cmdText, command.CommandText);

        Assert.NotNull(command.Parameters);
        if (data == null)
        {
            Assert.Equal(0, command.Parameters.Count);
        }
        else
        {
            Assert.Equal(1, command.Parameters.Count);
            Assert.Equal("@Data", command.Parameters[0].ParameterName);
            Assert.Equal(DbHelper.Normalize(data), command.Parameters[0].Value);
        }

        Assert.Equal(timeout ?? 30, command.CommandTimeout);

        Assert.Equal(expected, command.CommandType);
    }

    [Fact]
    public void CanStringBuilderToObj()
    {
        Assert.Null(((StringBuilder?)null).ToObj<string?>()); ;
        Assert.Null((new StringBuilder()).ToObj<string?>()); ;

        StringBuilder sb = new();
        sb.Append("t");
        sb.Append("r");
        sb.Append("u");
        sb.Append("e");

        Assert.True(sb.ToObj(false));
    }

    [Theory]
    [InlineData(null, null, null)]
    [InlineData(null, "A", "A")]
    [InlineData("", "A", "A")]
    [InlineData("A", "A", "A")]
    [InlineData("A", "B", "A")]
    public static void CanStringToObj(object? value, string? defaultValue = default, string? expected = default)
        => Assert.Equal(expected, value.ToObj(defaultValue));

    [Fact]
    public static void CanClassToObj()
    {
        var region = new Region() { Id = 1, Name = "West" };
        var actual = JsonSerializer.Serialize(region).ToObj<Region?>();

        Assert.Equal(region.Id, actual?.Id);
        Assert.Equal(region.Name, actual?.Name);

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(region)));
        actual = stream.ToObj<Region?>();

        Assert.Equal(region.Id, actual?.Id);
        Assert.Equal(region.Name, actual?.Name);
    }

    [Fact]
    public static void CanGuidToObj()
    {
        Assert.Equal((Guid?)null, (null as object).ToObj<Guid?>());
        Assert.Equal(Guid.Empty, (null as string).ToObj<Guid?>(Guid.Empty));
        Assert.Equal(Guid.Empty, string.Empty.ToObj<Guid?>(Guid.Empty));

        var someguid = Guid.NewGuid();
        var actual = someguid.ToString().ToObj<Guid?>();

        Assert.Equal(someguid, actual);
    }

    [Fact]
    public static void CanEnumToObj()
    {
        Assert.Equal((UserType?)null, (null as string).ToObj<UserType?>());
        Assert.Equal(UserType.Guest, (null as string).ToObj<UserType?>(UserType.Guest));
        Assert.Equal(UserType.Guest, UserType.Guest.ToString().ToObj<UserType?>());
        Assert.Equal(UserType.Guest, UserType.Guest.ToString("g").ToObj<UserType?>());
    }

    [Fact]
    public static void CanDateOnlyToObj()
    {
        Assert.Equal((DateOnly?)null, (null as string).ToObj<DateOnly?>());
        Assert.Equal(DateOnly.MinValue, (null as string).ToObj<DateOnly?>(DateOnly.MinValue));
        Assert.Equal(DateOnly.MinValue, DateOnly.MinValue.ToDateTime(TimeOnly.MinValue).ToObj<DateOnly?>());
    }

    [Fact]
    public static void CanTimeOnlyToObj()
    {
        Assert.Equal((TimeOnly?)null, (null as string).ToObj<TimeOnly?>());
        Assert.Equal(TimeOnly.MinValue, (null as string).ToObj<TimeOnly?>(TimeOnly.MinValue));
        Assert.Equal(TimeOnly.MinValue, TimeOnly.MinValue.ToTimeSpan().ToObj<TimeOnly?>());
    }

    [Fact]
    public static void CanPrimitiveToObj()
    {
        Assert.Equal((Int32?)null, (null as string).ToObj<Int32?>());
        Assert.Equal(Int32.MaxValue, (null as string).ToObj<Int32?>(Int32.MaxValue));
        Assert.Equal(Int32.MaxValue, Int32.MaxValue.ToString().ToObj<Int32?>());
    }

    [Fact]
    public static void CanOtherToObj()
    {
        Assert.Equal((DateTime?)null, (null as string).ToObj<DateTime?>());
        Assert.Equal(DateTime.MinValue, (null as string).ToObj<DateTime?>(DateTime.MinValue));
        Assert.Equal(DateTime.MinValue, DateTime.MinValue.ToString().ToObj<DateTime?>());
    }
}