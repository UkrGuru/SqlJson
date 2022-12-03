// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using SqlJsonTests;
using System.Reflection;
using UkrGuru.SqlJson;
using Xunit;

namespace UkrGuru.Extensions.Tests;

/// <summary>
/// 
/// </summary>
public class AssemblyExtensions
{
    public AssemblyExtensions()
    {
        DbHelper.ConnectionString = Globals.ConnectionString;
    }

    //[Fact]
    //public void CanExecResource()
    //{
    //    Assembly assembly = Assembly.GetExecutingAssembly();
    //    var assemblyName = assembly.GetName().Name;
    //    assembly.ExecResource($"{assemblyName}.Resources.SELECT_1.sql");
    //}

    //[Fact]
    //public async Task CanExecResourceAsync()
    //{
    //    Assembly assembly = Assembly.GetExecutingAssembly();
    //    var assemblyName = assembly.GetName().Name;
    //    await assembly.ExecResourceAsync($"{assemblyName}.Resources.SELECT_1.sql");
    //}

    [Fact]
    public void CanInitDb()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        assembly.InitDb();
    }
}