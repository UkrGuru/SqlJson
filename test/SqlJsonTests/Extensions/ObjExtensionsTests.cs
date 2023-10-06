// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text;
using System.Text.Json;
using static UkrGuru.SqlJson.GlobalTests;

namespace UkrGuru.SqlJson.Extensions;

public class ObjExtensionsTests
{
    [Fact]
    public void CanStringBuilderToObj()
    {
        Assert.Null(((StringBuilder?)null).ToObj<string?>()); ;
        Assert.Null(new StringBuilder().ToObj<string?>()); ;

        StringBuilder sb = new();
        sb.Append("t");
        sb.Append("r");
        sb.Append("u");
        sb.Append("e");

        Assert.True(sb.ToObj(false));
    }

    [Theory]
    [InlineData(null, null, null)]
    [InlineData(null, "A", "A")]
    [InlineData("", "A", "A")]
    [InlineData("A", "A", "A")]
    [InlineData("A", "B", "A")]
    public static void CanStringToObj(object? value, string? defaultValue = default, string? expected = default)
        => Assert.Equal(expected, value.ToObj(defaultValue));

    [Fact]
    public static void CanClassToObj()
    {
        var region = new Region() { Id = 1, Name = "West" };
        var actual = JsonSerializer.Serialize(region).ToObj<Region?>();

        Assert.Equal(region.Id, actual?.Id);
        Assert.Equal(region.Name, actual?.Name);

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(region)));
        actual = stream.ToObj<Region?>();

        Assert.Equal(region.Id, actual?.Id);
        Assert.Equal(region.Name, actual?.Name);
    }

    [Fact]
    public static void CanGuidToObj()
    {
        Assert.Null((null as object).ToObj<Guid?>());
        Assert.Equal(Guid.Empty, (null as string).ToObj<Guid?>(Guid.Empty));
        Assert.Equal(Guid.Empty, string.Empty.ToObj<Guid?>(Guid.Empty));

        var someguid = Guid.NewGuid();
        var actual = someguid.ToString().ToObj<Guid?>();

        Assert.Equal(someguid, actual);
    }

    [Fact]
    public static void CanEnumToObj()
    {
        Assert.Null((null as string).ToObj<UserType?>());
        Assert.Equal(UserType.Guest, (null as string).ToObj<UserType?>(UserType.Guest));
        Assert.Equal(UserType.Guest, UserType.Guest.ToString().ToObj<UserType?>());
        Assert.Equal(UserType.Guest, UserType.Guest.ToString("g").ToObj<UserType?>());
    }

    [Fact]
    public static void CanDateOnlyToObj()
    {
        Assert.Null((null as string).ToObj<DateOnly?>());
        Assert.Equal(DateOnly.MinValue, (null as string).ToObj<DateOnly?>(DateOnly.MinValue));
        Assert.Equal(DateOnly.MinValue, DateOnly.MinValue.ToDateTime(TimeOnly.MinValue).ToObj<DateOnly?>());
    }

    [Fact]
    public static void CanTimeOnlyToObj()
    {
        Assert.Null((null as string).ToObj<TimeOnly?>());
        Assert.Equal(TimeOnly.MinValue, (null as string).ToObj<TimeOnly?>(TimeOnly.MinValue));
        Assert.Equal(TimeOnly.MinValue, TimeOnly.MinValue.ToTimeSpan().ToObj<TimeOnly?>());
    }

    [Fact]
    public static void CanPrimitiveToObj()
    {
        Assert.Null((null as string).ToObj<int?>());
        Assert.Equal(int.MaxValue, (null as string).ToObj<int?>(int.MaxValue));
        Assert.Equal(int.MaxValue, int.MaxValue.ToString().ToObj<int?>());
    }

    [Fact]
    public static void CanOtherToObj()
    {
        Assert.Null((null as string).ToObj<DateTime?>());
        Assert.Equal(DateTime.MinValue, (null as string).ToObj<DateTime?>(DateTime.MinValue));
        //Assert.Equal(DateTime.MinValue, DateTime.MinValue.ToString().ToObj<DateTime?>());
    }
}