// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using System.Data;
using static UkrGuru.SqlJson.GlobalTests;

namespace UkrGuru.SqlJson;

public class DbExtensionsTests
{
    [Fact]
    public void IsLong_FactTests()
    {
        Assert.False(DbExtensions.IsLong<bool?>());
        Assert.False(DbExtensions.IsLong<bool>());

        Assert.False(DbExtensions.IsLong<byte?>());
        Assert.False(DbExtensions.IsLong<byte>());

        Assert.False(DbExtensions.IsLong<bool?>());
        Assert.False(DbExtensions.IsLong<bool>());

        Assert.False(DbExtensions.IsLong<short?>());
        Assert.False(DbExtensions.IsLong<short>());

        Assert.False(DbExtensions.IsLong<int?>());
        Assert.False(DbExtensions.IsLong<int>());

        Assert.False(DbExtensions.IsLong<long?>());
        Assert.False(DbExtensions.IsLong<long>());

        Assert.False(DbExtensions.IsLong<float?>());
        Assert.False(DbExtensions.IsLong<float>());

        Assert.False(DbExtensions.IsLong<double?>());
        Assert.False(DbExtensions.IsLong<double>());

        Assert.False(DbExtensions.IsLong<decimal?>());
        Assert.False(DbExtensions.IsLong<decimal>());

        Assert.False(DbExtensions.IsLong<DateOnly?>());
        Assert.False(DbExtensions.IsLong<DateOnly>());

        Assert.False(DbExtensions.IsLong<DateTime?>());
        Assert.False(DbExtensions.IsLong<DateTime>());

        Assert.False(DbExtensions.IsLong<DateTimeOffset?>());
        Assert.False(DbExtensions.IsLong<DateTimeOffset>());

        Assert.False(DbExtensions.IsLong<TimeOnly?>());
        Assert.False(DbExtensions.IsLong<TimeOnly>());

        Assert.False(DbExtensions.IsLong<TimeSpan?>());
        Assert.False(DbExtensions.IsLong<TimeSpan>());

        Assert.False(DbExtensions.IsLong<Guid?>());
        Assert.False(DbExtensions.IsLong<Guid>());

        Assert.False(DbExtensions.IsLong<char?>());
        Assert.False(DbExtensions.IsLong<char>());

        Assert.False(DbExtensions.IsLong<UserType?>());
        Assert.False(DbExtensions.IsLong<UserType>());

        Assert.True(DbExtensions.IsLong<string?>());
        Assert.True(DbExtensions.IsLong<string>());

        Assert.True(DbExtensions.IsLong<byte[]?>());
        Assert.True(DbExtensions.IsLong<byte[]>());

        Assert.True(DbExtensions.IsLong<char[]?>());
        Assert.True(DbExtensions.IsLong<char[]>());

        Assert.True(DbExtensions.IsLong<Stream>());
        Assert.True(DbExtensions.IsLong<TextReader>());
        Assert.True(DbExtensions.IsLong<object>());
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData(typeof(bool), false)]
    [InlineData(typeof(byte), false)]
    [InlineData(typeof(short), false)]
    [InlineData(typeof(int), false)]
    [InlineData(typeof(long), false)]
    [InlineData(typeof(float), false)]
    [InlineData(typeof(double), false)]
    [InlineData(typeof(decimal), false)]
    [InlineData(typeof(DateOnly), false)]
    [InlineData(typeof(DateTime), false)]
    [InlineData(typeof(DateTimeOffset), false)]
    [InlineData(typeof(TimeOnly), false)]
    [InlineData(typeof(TimeSpan), false)]
    [InlineData(typeof(Guid), false)]
    [InlineData(typeof(char), false)]
    [InlineData(typeof(UserType), false)]
    [InlineData(typeof(string), true)]
    [InlineData(typeof(byte[]), true)]
    [InlineData(typeof(char[]), true)]
    [InlineData(typeof(Stream), true)]
    [InlineData(typeof(TextReader), true)]
    [InlineData(typeof(object), true)]
    public void IsLong_TheoryTests(Type? t, bool expected)
        => Assert.Equal(expected, t.IsLong());

    [Theory]
    [InlineData("SELECT @Data", null)]
    [InlineData("SELECT @Data", 1)]
    [InlineData("SELECT @Data", null, 15)]
    [InlineData("SELECT @Data", null, 45)]
    [InlineData("ProcTest", null, null, CommandType.StoredProcedure)]
    public static void CanCreateSqlCommand(string cmdText, object? data = null, int? timeout = null, CommandType expected = CommandType.Text)
    {
        // Arrange
        SqlConnection connection = new SqlConnection(ConnectionString);

        // Act
        var command = connection.CreateSqlCommand(cmdText, data, timeout);

        // Assert
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
    public static void CanConvertScalar()
    {
        Assert.Equal((bool?)null, DbExtensions.ConvertScalar<bool?>(null));
        Assert.Equal((bool?)null, DbExtensions.ConvertScalar<bool?>(DBNull.Value));

        Assert.Equal(true, DbExtensions.ConvertScalar<bool?>(true));
        Assert.Equal(false, DbExtensions.ConvertScalar<bool?>(false));

        Assert.Null(DbExtensions.ConvertScalar<byte?>(null));
        Assert.Equal(byte.MaxValue, DbExtensions.ConvertScalar<byte?>(byte.MaxValue));

        Assert.Null(DbExtensions.ConvertScalar<short?>(null));
        Assert.Equal(short.MaxValue, DbExtensions.ConvertScalar<short?>(short.MaxValue));

        Assert.Null(DbExtensions.ConvertScalar<int?>(null));
        Assert.Equal(int.MaxValue, DbExtensions.ConvertScalar<int?>(int.MaxValue));

        Assert.Null(DbExtensions.ConvertScalar<long?>(null));
        Assert.Equal(long.MaxValue, DbExtensions.ConvertScalar<long?>(long.MaxValue));

        Assert.Null(DbExtensions.ConvertScalar<float?>(null));
        Assert.Equal(float.MaxValue, DbExtensions.ConvertScalar<float?>(float.MaxValue));

        Assert.Null(DbExtensions.ConvertScalar<double?>(null));
        Assert.Equal(double.MaxValue, DbExtensions.ConvertScalar<double?>(double.MaxValue));

        Assert.Null(DbExtensions.ConvertScalar<decimal?>(null));
        Assert.Equal(decimal.MaxValue, DbExtensions.ConvertScalar<decimal?>(decimal.MaxValue));

        Assert.Null(DbExtensions.ConvertScalar<DateOnly?>(null));
        Assert.Equal(DateOnly.MaxValue, DbExtensions.ConvertScalar<DateOnly?>(DateOnly.MaxValue.ToDateTime(TimeOnly.MinValue)));

        Assert.Null(DbExtensions.ConvertScalar<DateTime?>(null));
        Assert.Equal(DateTime.MaxValue, DbExtensions.ConvertScalar<DateTime?>(DateTime.MaxValue));

        Assert.Null(DbExtensions.ConvertScalar<DateTimeOffset?>(null));
        Assert.Equal(DateTimeOffset.MaxValue, DbExtensions.ConvertScalar<DateTimeOffset?>(DateTimeOffset.MaxValue));

        Assert.Null(DbExtensions.ConvertScalar<TimeOnly?>(null));
        Assert.Equal(TimeOnly.MaxValue, DbExtensions.ConvertScalar<TimeOnly?>(TimeOnly.MaxValue.ToTimeSpan()));

        Assert.Null(DbExtensions.ConvertScalar<TimeSpan?>(null));
        Assert.Equal(TimeSpan.MaxValue, DbExtensions.ConvertScalar<TimeSpan?>(TimeSpan.MaxValue));

        Assert.Null(DbExtensions.ConvertScalar<Guid?>(null));
        Assert.Equal(Guid.Empty, DbExtensions.ConvertScalar<Guid?>(Guid.Empty));

        Assert.Null(DbExtensions.ConvertScalar<char?>(null));
        Assert.Equal('x', DbExtensions.ConvertScalar<char?>('x'.ToString()));

        Assert.Null(DbExtensions.ConvertScalar<string?>(null));
        Assert.Equal(string.Empty, DbExtensions.ConvertScalar<string?>(string.Empty));
    }
}