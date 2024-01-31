// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Data.SqlTypes;
using System.Xml.Linq;
using static UkrGuru.SqlJson.Tests.GlobalTests;

namespace UkrGuru.SqlJson.Tests;

public class XTests
{
    private readonly IDbService _db;

    public XTests() => _db = new ApiDbService(Http);


    public static IEnumerable<object[]> GetTestString(int numTests)
    {
        var allData = new List<object[]>
        {
            new object[] { string.Empty },
            new object[] { TestString1k },
            new object[] { TestString5k },
            new object[] { TestString55k }
        };

        return allData.Take(numTests);
    }

    [Theory]
    [MemberData(nameof(GetTestString), parameters: 1)]
    public async Task CanExecAsync_SqlXml(string text)
    {
        var value = string.IsNullOrEmpty(text) ? "<value />" : new XElement("value", text).ToString();

        var sqlValue = new SqlXml(new System.Xml.XmlTextReader(new StringReader(value)));

        var sqlActual = await _db.ExecAsync<SqlXml>("ProcXml", sqlValue);

        Assert.NotNull(sqlActual);
        Assert.Equal(sqlActual.Value, sqlValue.Value);
    }
}