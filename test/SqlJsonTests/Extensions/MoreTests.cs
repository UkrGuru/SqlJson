// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text.Json;

namespace UkrGuru.SqlJson.Extensions.Tests;

public class MoreTests
{
    [Fact]
    public void CanAddNew()
    {
        More more = new();
        more.AddNew(null);
        more.AddNew("");
        more.AddNew("{}");
        more.AddNew(JsonSerializer.Serialize(new
        {
            dataNull = null as string,
            dataEmpty = string.Empty,
            dataNormal = "asd",
            boolNull = (bool?)null,
            boolTrue = true,
            boolFalse = false,
        }));

        var files = new[] { "file1.txt", "file2.txt" };
        more.AddNew(JsonSerializer.Serialize(new { type = "Action", timeout = 60, amount = 123.45, files }));

        Assert.Null(more.GetValue("dataNull"));
        Assert.Empty(more.GetValue("dataEmpty")!);
        Assert.Equal("asd", more.GetValue("dataNormal"));
        Assert.Null(more.GetValue("dataNotExists"));

        Assert.True(more.GetValue<bool>("boolTrue"));
        Assert.False(more.GetValue<bool>("boolFalse"));
        Assert.Null(more.GetValue<bool?>("boolNull"));

        Assert.True(more.GetValue("enabled", true));
        Assert.False(more.GetValue("enabled", false));
        Assert.Null(more.GetValue("enabled"));

        Assert.Equal(60, more.GetValue("timeout", 0));
        Assert.Equal(123.45, more.GetValue("amount", 0.0));

        var actualFiles = more.GetValue<object[]?>("files");
        Assert.NotNull(actualFiles);
        Assert.Equal(2, actualFiles.Length);
        Assert.Equal(files[0], Convert.ToString(actualFiles[0]));
        Assert.Equal(files[1], Convert.ToString(actualFiles[1]));
    }
}