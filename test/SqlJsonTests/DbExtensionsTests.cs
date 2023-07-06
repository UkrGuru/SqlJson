// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using System.Data;
using static SqlJsonTests.Extensions.SqlObjectExtensionsTests;

namespace UkrGuru.SqlJson;

public class DbExtensionsTests
{
//    [Theory]
//    [InlineData(null, false)]
//    [InlineData(typeof(Boolean), false)]
//    [InlineData(typeof(Byte), false)]
//    [InlineData(typeof(Int16), false)]
//    [InlineData(typeof(Int32), false)]
//    [InlineData(typeof(Int64), false)]
//    [InlineData(typeof(Single), false)]
//    [InlineData(typeof(Double), false)]
//    [InlineData(typeof(Decimal), false)]
//    [InlineData(typeof(DateOnly), false)]
//    [InlineData(typeof(DateTime), false)]
//    [InlineData(typeof(DateTimeOffset), false)]
//    [InlineData(typeof(TimeOnly), false)]
//    [InlineData(typeof(TimeSpan), false)]
//    [InlineData(typeof(Guid), false)]
//    [InlineData(typeof(Char), false)]
//    [InlineData(typeof(UserType), false)]
//    [InlineData(typeof(Byte[]), true)]
//    [InlineData(typeof(Char[]), true)]
//    public void IsLongTests(Type? t, bool expected)
//        => Assert.Equal(expected, t.IsLong());

//    [Theory]
//    [InlineData("SELECT 1;", null)]
//    [InlineData("SELECT 1;", 1)]
//    [InlineData("SELECT 1;", null, 15)]
//    [InlineData("SELECT 1;", null, 45)]
//    [InlineData("ProcTest", null, null, CommandType.StoredProcedure)]
//    public static void CanCreateSqlCommand(string cmdText, object? data = null, int? timeout = null, CommandType expected = CommandType.Text)
//    {
//        using var connection = new SqlConnection(GlobalTests.ConnectionString);
//        connection.Open();

//        var command = connection.CreateSqlCommand(cmdText, data, timeout);

//        Assert.NotNull(command);
//        Assert.Equal(cmdText, command.CommandText);

//        Assert.NotNull(command.Parameters);
//        if (data == null)
//        {
//            Assert.Equal(0, command.Parameters.Count);
//        }
//        else
//        {
//            Assert.Equal(1, command.Parameters.Count);
//            Assert.Equal("@Data", command.Parameters[0].ParameterName);
//            Assert.Equal(DbHelper.Normalize(data), command.Parameters[0].Value);
//        }

//        Assert.Equal(timeout ?? 30, command.CommandTimeout);

//        Assert.Equal(expected, command.CommandType);
//    }
}