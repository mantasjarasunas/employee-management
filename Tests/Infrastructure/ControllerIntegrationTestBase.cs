using System.Net;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Xunit;

namespace Tests.Infrastructure
{
    [Collection("Context collection")]
    public class ControllerIntegrationTestBase : IClassFixture<AppFactory>
    {
        private readonly AppFactory _factory;
        protected readonly HttpClient _client;

        public ControllerIntegrationTestBase(AppFactory fixture)
        {
            _factory = fixture;
            _client = _factory.CreateClient();
        }
        
        protected async Task<T?> ApiPost<T>(string url, object postObject)
        {
            var httpResponse = await _client.PostAsync(url, CreatePostObject(postObject));
            httpResponse.EnsureSuccessStatusCode();
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(stringResponse);
        }

        protected async Task<T?> ApiPut<T>(string url, object postObject)
        {
            var httpResponse = await _client.PutAsync(url, CreatePostObject(postObject));
            httpResponse.EnsureSuccessStatusCode();
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(stringResponse);
        }

        protected async Task<T?> ApiGet<T>(string url)
        {
            var httpResponse = await _client.GetAsync(url);
            httpResponse.EnsureSuccessStatusCode();
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(stringResponse);
        }
        
        protected async Task<ValidationProblemDetails> ApiPostReturnsValidationErrors(string url, object postObject)
        {
            var httpResponse = await _client.PostAsync(url, CreatePostObject(postObject));
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(stringResponse)!;
            return problemDetails;
        }

        private static StringContent CreatePostObject(object data)
        {
            return new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        }
    }
}
