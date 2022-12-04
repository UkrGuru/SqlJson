// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using SqlJsonTests;
using System.Reflection;
using UkrGuru.SqlJson;
using Xunit;

namespace UkrGuru.Extensions;

public class AssemblyExtensionsTests
{
    public AssemblyExtensionsTests() => DbHelper.ConnectionString = Globals.ConnectionString;

    [Fact]
    public void CanInitDb() => Assembly.GetExecutingAssembly().InitDb();
}