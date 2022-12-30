﻿ // Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using UkrGuru.Extensions;

namespace SqlJsonTests.Extensions;

public class StringExtensionsTests
{
    //[Fact]
    //public void CanStringBuilderToObj()
    //{

    //}

    [Fact]
    public void CanStringToObj()
    {
        var b1 = ((string?)null).ToObj<bool?>();
        var b2 = "".ToObj<bool?>();
        var b3 = "true".ToObj<bool>();

        var n0 = ((string?)null).ToObj<int?>();
        var n1 = "123".ToObj<int?>();

        var g0 = ((string?)null).ToObj<Guid?>();
        var g1 = Guid.NewGuid().ToString().ToObj<Guid>();

        var s0 = ((string?)null).ToObj<string?>();
        var s1 = "true".ToObj<string>();

        var d0 = ((string?)null).ToObj<DateTime?>();
        var d1 = "Feb 17 2022 11:58AM".ToObj<DateTime>();

        var r0 = ((string?)null).ToObj<Region?>();
        var r1 = @"{ ""Id"" : 1 }".ToObj<Region>();
    }

    private class Region
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}