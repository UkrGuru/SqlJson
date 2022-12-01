using System.Text.Json;
using UkrGuru.SqlJson;
using Xunit;

namespace SqlJsonTests;

public class DbHelper_NormalizeParamsTests
{
    [Fact]
    public void DbHelper_Normalize_Tests()
    {
        object data = "str1";
        Assert.Equal(data, DbHelper.NormalizeParams(data));

        data = true;
        Assert.Equal(data, DbHelper.NormalizeParams(data));
        data = false;
        Assert.Equal(data, DbHelper.NormalizeParams(data));

        data = (byte)1;
        Assert.Equal(data, DbHelper.NormalizeParams(data));
        data = new byte[] { 1, 2 };
        Assert.Equal(data, DbHelper.NormalizeParams(data));

        data = new char[] { '1', '2' };
        Assert.Equal(data, DbHelper.NormalizeParams(data));

        data = new DateTime(2000, 1, 1);
        Assert.Equal(data, DbHelper.NormalizeParams(data));

        data = new DateTimeOffset(new DateTime(2000, 1, 1));
        Assert.Equal(data, DbHelper.NormalizeParams(data));

        data = (decimal)123.45;
        Assert.Equal(data, DbHelper.NormalizeParams(data));

        data = (double)123.45;
        Assert.Equal(data, DbHelper.NormalizeParams(data));

        data = Guid.NewGuid();
        Assert.Equal(data, DbHelper.NormalizeParams(data));
        
        data = (Int16)1;
        Assert.Equal(data, DbHelper.NormalizeParams(data));

        data = (Int32)1;
        Assert.Equal(data, DbHelper.NormalizeParams(data));

        data = new { Name = "Proc1" };
        Assert.Equal(JsonSerializer.Serialize(data), DbHelper.NormalizeParams(data));

        data = JsonSerializer.Serialize(new { Name = "Proc1" });
        Assert.Equal(data, DbHelper.NormalizeParams(JsonSerializer.Deserialize<dynamic?>(Convert.ToString(data)!)));
    }
}

