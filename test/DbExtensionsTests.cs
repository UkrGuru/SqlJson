// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using System.Data;

namespace UkrGuru.SqlJson;

public class DbExtensionsTests
{
    //[Theory]
    //[InlineData(null, true)]
    //[InlineData("", true)]
    //[InlineData(typeof(bool), false)]
    //[InlineData("Byte", false)]
    //[InlineData("DateTime", false)]
    //[InlineData("DateTimeOffset", false)]
    //[InlineData("Decimal", false)]
    //[InlineData("Double", false)]
    //[InlineData("Guid", false)]
    //[InlineData("Int16", false)]
    //[InlineData("Int32", false)]
    //[InlineData("Single", false)]
    //[InlineData("TimeSpan", false)]
    //[InlineData("Byte[]", true)]
    //[InlineData("Char[]", true)]
    //[InlineData("Xml", true)]
    //public void IsLongTests(Type t, bool expected)


    //    => Assert.Equal(expected, DbHelper.IsLong<t>());
    //[Theory]
    //[InlineData("SELECT 1;", null)]
    //[InlineData("SELECT 1;", 1)]
    //[InlineData("SELECT 1;", null, 15)]
    //[InlineData("SELECT 1;", null, 45)]
    //[InlineData("ProcTest", null, null, CommandType.StoredProcedure)]
    //public static void CanCreateSqlCommand(string cmdText, object? data = null, int? timeout = null, CommandType expected = CommandType.Text)
    //{
    //    using var connection = new SqlConnection(GlobalTests.ConnectionString);
    //    connection.Open();

    //    var command = connection.CreateSqlCommand(cmdText, data, timeout);

    //    Assert.NotNull(command);
    //    Assert.Equal(cmdText, command.CommandText);

    //    Assert.NotNull(command.Parameters);
    //    if (data == null)
    //    {
    //        Assert.Equal(0, command.Parameters.Count);
    //    }
    //    else
    //    {
    //        Assert.Equal(1, command.Parameters.Count);
    //        Assert.Equal("@Data", command.Parameters[0].ParameterName);
    //        Assert.Equal(DbHelper.Normalize(data), command.Parameters[0].Value);
    //    }

    //    Assert.Equal(timeout ?? 30, command.CommandTimeout);

    //    Assert.Equal(expected, command.CommandType);
    //}
}