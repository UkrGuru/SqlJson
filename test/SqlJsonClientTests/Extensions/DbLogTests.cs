namespace UkrGuru.SqlJson.Extensions.Tests;

public class DbLogTests
{
    [Fact]
    public void DbLog_Logged_ShouldBeSet()
    {
        var dbLog = new DbLog();
        var expectedLoggedDate = DateTime.Now;

        dbLog.Logged = expectedLoggedDate;

        Assert.Equal(expectedLoggedDate, dbLog.Logged);
    }

    [Fact]
    public void DbLog_LogLevel_ShouldBeSet()
    {
        var dbLog = new DbLog();
        var expectedLogLevel = DbLogLevel.Information;

        dbLog.LogLevel = expectedLogLevel;

        Assert.Equal(expectedLogLevel, dbLog.LogLevel);
    }

    [Fact]
    public void DbLog_Title_ShouldBeSet()
    {
        var dbLog = new DbLog();
        var expectedTitle = "Error occurred";

        dbLog.Title = expectedTitle;

        Assert.Equal(expectedTitle, dbLog.Title);
    }

    [Fact]
    public void DbLog_LogMore_ShouldBeSet()
    {
        var dbLog = new DbLog();
        var expectedLogMore = new { Message = "Something happened", Details = "Additional info" };

        dbLog.LogMore = expectedLogMore;

        Assert.Equal(expectedLogMore, dbLog.LogMore);
    }
}
