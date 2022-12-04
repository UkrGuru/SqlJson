// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace UkrGuru.SqlJson;

public class DbExtensionsTests
{
    //[Theory]
    //[InlineData("SELECT 1", null)]
    //[InlineData("SELECT 1", 1)]
    //[InlineData("SELECT 1", null, 15)]
    //[InlineData("SELECT 1", null, 45)]
    //[InlineData("ProcTest", null, null, CommandType.StoredProcedure)]
    //public static void CreateSqlCommandTests(string cmdText, object? data = null, int? timeout = null, CommandType type = CommandType.Text)
    //{
    //    using var connection = new SqlConnection(ConnectionString);
    //    connection.Open();

    //    var command = connection.CreateSqlCommand(cmdText, data, timeout, type);

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
    //        Assert.Equal(DbHelper.NormalizeParams(data), command.Parameters[0].Value);
    //    }

    //    Assert.Equal(timeout ?? 30, command.CommandTimeout);

    //    Assert.Equal(type, command.CommandType);
    //}
}