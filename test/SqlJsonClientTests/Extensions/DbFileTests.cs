namespace UkrGuru.SqlJson.Extensions.Tests;

public class DbFileTests
{
    [Fact]
    public void DbFile_Created_ShouldBeSet()
    {
        var dbFile = new DbFile();
        var expectedCreatedDate = DateTime.Now;

        dbFile.Created = expectedCreatedDate;

        Assert.Equal(expectedCreatedDate, dbFile.Created);
    }

    [Fact]
    public void DbFile_FileName_ShouldBeSet()
    {
        var dbFile = new DbFile();
        var expectedFileName = "example.txt";

        dbFile.FileName = expectedFileName;

        Assert.Equal(expectedFileName, dbFile.FileName);
    }

    [Fact]
    public void DbFile_FileContent_ShouldBeSet()
    {
        var dbFile = new DbFile();
        var expectedContent = new byte[] { 0x41, 0x42, 0x43 };

        dbFile.FileContent = expectedContent;

        Assert.Equal(expectedContent, dbFile.FileContent);
    }

    [Fact]
    public void DbFile_Safe_ShouldDefaultToFalse()
    {
        var dbFile = new DbFile();

        Assert.False(dbFile.Safe);
    }
}