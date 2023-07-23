// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Net;
using static UkrGuru.SqlJson.GlobalTests;

namespace UkrGuru.SqlJson;

public class ApiExtensionsTests
{
    [Fact]
    public async Task CanReadAsync()
    {
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);

        httpResponse.Content = new StringContent("");
        var result = await httpResponse.ReadAsync();
        Assert.Equal(1, result);

        httpResponse = new HttpResponseMessage(HttpStatusCode.RequestTimeout);
        await Assert.ThrowsAsync<HttpRequestException>(() => httpResponse.ReadAsync());
    }


    [Fact]
    public async Task CanReadAsyncToObj()
    {
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);

        httpResponse.Content = new StringContent(""" { "Id" : 1, "Name": "Name1" } """);

        var result = await httpResponse.ReadAsync<Region>();
        Assert.IsType<Region>(result);

        httpResponse = new HttpResponseMessage(HttpStatusCode.RequestTimeout);
        await Assert.ThrowsAsync<HttpRequestException>(() => httpResponse.ReadAsync<Region>());
    }

    [Fact]
    public void CanThrowIfError()
    {
        string content = "Error: Test Error Message";

        var exception = Assert.Throws<HttpRequestException>(() => content.ThrowIfError());

        Assert.Equal("Test Error Message", exception.Message);
    }
}
