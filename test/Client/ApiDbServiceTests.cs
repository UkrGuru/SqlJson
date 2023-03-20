// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Azure.Core;
using Azure;
using System.Text.Json;
using WireMock.Server;

namespace UkrGuru.SqlJson.Client;

public class ApiDbServiceTests : IDisposable
{
    private WireMockServer _server;
    public ApiDbServiceTests()
    {
        _server = WireMockServer.Start();

    }

    public void Dispose()
    {
        _server.Stop();
    }

    [Fact]
    public async Task UsingWireMock()
    {
        //const int personId = 1;
        //var dto = new Person { Id = personId, Name = "Dave", Age = 42 };
        //var mockUrl = $"https://myurl?id={personId}";
        //var serializedDto = JsonSerializer.Serialize(dto);
        //_server.Given(
        //    Request.Create()
        //        .WithPath("/"))
        //    .RespondWith(
        //        Response.Create()
        //            .WithStatusCode(200)
        //            .WithHeader("Content-Type", "application/json")
        //            .WithBodyAsJson(serializedDto));

        //var httpClient = HttpClientFactory.Create();
        //var sut = new ApiDbService(httpClient);

        //var actual = await sut.GetPersonAsync(personId);

        //Assert.NotNull(actual);
        //Assert.Equivalent(dto, actual);

        await Task.CompletedTask;
    }
}