namespace UkrGuru.SqlJson.Extensions.Tests;

public class DbLogExtensionsTests
{
    [Fact]
    public void Normalize_ShouldCreateValidDbLog()
    {
        var logLevel = DbLogLevel.Warning;
        var title = "Sample Log";
        var additionalInfo = new { Message = "Something happened", Details = "Additional info" };

        var normalizedLog = DbLogExtensions.Normalize(logLevel, title, additionalInfo);

        Assert.NotNull(normalizedLog);
        Assert.IsType<DbLog>(normalizedLog);
        var dbLog = (DbLog)normalizedLog;
        Assert.Equal(logLevel, dbLog.LogLevel);
        Assert.Equal(title, dbLog.Title);
        Assert.Equal(additionalInfo, dbLog.LogMore);
    }

    [Fact]
    public void MinDbLogLevel_ShouldDefaultToInformation()
    {
        var defaultMinLogLevel = DbLogLevel.Information;

        var minLogLevel = DbLogExtensions.MinDbLogLevel;

        Assert.Equal(defaultMinLogLevel, minLogLevel);
    }
}
