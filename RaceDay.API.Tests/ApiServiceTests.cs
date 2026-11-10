using RaceDay.MVC.Services;
using System.Net;
using System.Text;
using Xunit;

namespace RaceDay.API.Tests;

public sealed class ApiServiceTests
{
    [Fact]
    public async Task PutAsync_SendsJsonAndBearerToken()
    {
        HttpRequestMessage? capturedRequest = null;
        string? capturedBody = null;
        var handler = new StubHttpMessageHandler(request =>
        {
            capturedRequest = request;
            capturedBody = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    "{\"firstName\":\"Alex\"}", Encoding.UTF8, "application/json")
            };
        });
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.example.test/")
        };
        var service = new ApiService(client);

        var result = await service.PutAsync<object, ProfileResponse>(
            "api/Participants/me", new { FirstName = "Alex" }, "test-jwt");

        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Put, capturedRequest.Method);
        Assert.Equal("Bearer", capturedRequest.Headers.Authorization?.Scheme);
        Assert.Equal("test-jwt", capturedRequest.Headers.Authorization?.Parameter);
        Assert.Contains("Alex", capturedBody);
        Assert.Equal("Alex", result?.FirstName);
    }

    private sealed class ProfileResponse
    {
        public string FirstName { get; set; } = string.Empty;
    }

    private sealed class StubHttpMessageHandler(
        Func<HttpRequestMessage, HttpResponseMessage> responder) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(responder(request));
        }
    }
}
