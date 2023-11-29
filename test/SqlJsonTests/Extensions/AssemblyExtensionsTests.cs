using System.Reflection;
using static UkrGuru.SqlJson.Tests.GlobalTests;

namespace UkrGuru.SqlJson.Extensions.Tests;

public class AssemblyExtensionsTests
{
    public AssemblyExtensionsTests()
    {
        DbHelper.ConnectionString = ConnectionString;
    }

    [Fact]
    public void CanExecResource()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceFullName = $"{assembly.GetName().Name}.Resources.TestProc.sql";

        try
        {
            DbHelper.Exec("DROP PROCEDURE IF EXISTS TestProc");

            assembly.ExecResource(resourceFullName);

            Assert.Equal("OK", DbHelper.Exec<string>("TestProc"));
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }

    [Fact]
    public async Task CanExecResourceAsync()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceFullName = $"{assembly.GetName().Name}.Resources.TestProcAsync.sql";

        try
        {
            await DbHelper.ExecAsync("DROP PROCEDURE IF EXISTS TestProcAsync");

            await assembly.ExecResourceAsync(resourceFullName);

            Assert.Equal("OK", DbHelper.Exec<string>("TestProcAsync"));

        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
        finally { await Task.CompletedTask; }
    }
}
