// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text.Json;

namespace UkrGuru.SqlJson.Extensions.Tests;

public class MoreTests
{
    [Fact]
    public void CanAddNew_Null()
    {
        More more = new();

        more.AddNew(default);

        Assert.Empty(more);
    }

    [Fact]
    public void CanAddNew_Empty()
    {
        More more = new();

        more.AddNew(string.Empty);

        Assert.Empty(more);
    }

    [Fact]
    public void CanAddNew_Whitespace()
    {
        More more = new();

        more.AddNew("   ");

        Assert.Empty(more);
    }

    [Fact]
    public void CanAddNew_NotBlank()
    {
        More more = new(); 

        more.AddNew(@"null");

        Assert.Empty(more);
    }

    [Fact]
    public void CanAddNew_Key_0_2()
    {
        More more = new();

        more.AddNew(@"{ ""key1"" : null }");
        more.AddNew(@"{ ""key1"" : 2 }");

        Assert.Single(more);
        Assert.Null(more.GetValue<int?>("key1"));
    }

    [Fact]
    public void CanAddNew_Key_1_1()
    {
        More more = new();

        more.AddNew(@"{ ""key1"" : 1  }");

        Assert.Single(more);
        Assert.Equal(1, more.GetValue<int?>("key1"));
    }

    [Fact]
    public void CanAddNew_Key_1_2()
    {
        More more = new();

        more.AddNew(@"{ ""key1"" : 1 }");
        more.AddNew(@"{ ""key1"" : 2 }");

        Assert.Single(more);
        Assert.Equal(1, more.GetValue<int?>("key1"));
    }

    [Fact]
    public void CanAddNew_Key_2_2()
    {
        More more = new();
        more.AddNew(@"{ ""key1"" : 1 }");
        more.AddNew(@"{ ""key1"" : 2, ""key2"" : 2 }");

        Assert.Equal(2, more.Count);
        Assert.Equal(1, more.GetValue<int?>("key1"));
        Assert.Equal(2, more.GetValue<int?>("key2"));
    }

    [Fact]
    public void CanGetValue_Value()
    {
        More more = new(); 
        more.AddNew(@"{ ""boolNull"" : null, ""boolTrue"" : true, ""boolFalse"" : false } ");

        Assert.Null(more.GetValue<bool?>("boolNull"));
        Assert.True(more.GetValue<bool?>("boolTrue"));
        Assert.False(more.GetValue<bool?>("boolFalse"));

        var files = new[] { "file1.txt", "file2.txt" };
        more.Clear(); more.AddNew(JsonSerializer.Serialize(new { files }));

        var actualFiles = more.GetValue<object[]?>("files");
        Assert.NotNull(actualFiles);
        Assert.Equal(2, actualFiles.Length);
        Assert.Equal(files[0], Convert.ToString(actualFiles[0]));
        Assert.Equal(files[1], Convert.ToString(actualFiles[1]));

        more.Clear(); more["keyGuid"] = Guid.Empty;
        //var value = more.GetValue("keyGuid");
        Assert.Single(more);
        Assert.Equal(Guid.Empty.ToString(), more.GetValue("keyGuid"));
    }

    [Fact]
    public void CanGetValue_Default()
    {
        More more = new();

        Assert.Null(more.GetValue<bool?>("boolNull"));
        Assert.True(more.GetValue<bool?>("boolTrue", true));
        Assert.False(more.GetValue<bool?>("boolFalse", false));
    }

    [Fact]
    public void CanSetValue()
    {
        More more = new();
        Assert.Null(more.GetValue<bool?>("key1"));

        more["key1"] = null;
        Assert.Null(more.GetValue<bool?>("key1"));

        more["key1"] = true;
        Assert.True(more.GetValue<bool?>("key1"));

        more["key1"] = false;
        Assert.False(more.GetValue<bool?>("key1"));
    }
}