using Microsoft.Data.SqlClient;
using System.Data;
using UkrGuru.SqlJson;
using Xunit;

namespace SqlJsonTests;

public class DbTests
{
    private static string DbName => new SqlConnectionStringBuilder(ConnectionString).InitialCatalog;

    public DbTests()
    {
        DbHelper.ConnectionString = ConnectionString.Replace(DbName, "master");

        DbHelper.ExecCommand($"IF DB_ID('{DbName}') IS NULL CREATE DATABASE {DbName};");

        DbHelper.ConnectionString = ConnectionString;

        if (dbOK) return;

        dbOK = true;
    }

    [Fact]
    public static void CreateSqlConnectionTests()
    {
        DbHelper.ConnectionString = ConnectionString;
        var connection = DbHelper.CreateSqlConnection();

        Assert.NotNull(connection);
        Assert.Equal(DbName, connection.Database);
        Assert.Equal(ConnectionString, connection.ConnectionString);
    }

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