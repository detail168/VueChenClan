using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Bulky.IntegrationTests
{
    public class ApiControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;

        public ApiControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetCategory_ReturnsOkStatusAndData()
        {
            // Arrange
            var url = "/api/admin/category";

            // Act
            var response = await _client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content);
            Assert.NotEmpty(content);
            
            var doc = JsonDocument.Parse(content);
            Assert.True(doc.RootElement.TryGetProperty("data", out var dataElement));
            var dataArray = dataElement;
            Assert.True(dataArray.GetArrayLength() > 0);
        }

        [Fact]
        public async Task GetProduct_ReturnsOkStatusAndData()
        {
            // Arrange
            var url = "/api/admin/product";

            // Act
            var response = await _client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content);
            Assert.NotEmpty(content);
            
            var doc = JsonDocument.Parse(content);
            Assert.True(doc.RootElement.TryGetProperty("data", out var dataElement));
            var dataArray = dataElement;
            Assert.True(dataArray.GetArrayLength() > 0);
        }

        [Fact]
        public async Task GetKindness_ReturnsOkStatusAndData()
        {
            // Arrange
            var url = "/api/admin/kindness";

            // Act
            var response = await _client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content);
            Assert.NotEmpty(content);
            
            var doc = JsonDocument.Parse(content);
            Assert.True(doc.RootElement.TryGetProperty("data", out _));
        }

        [Fact]
        public async Task GetAncestral_ReturnsOkStatusAndData()
        {
            // Arrange
            var url = "/api/admin/ancestral";

            // Act
            var response = await _client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content);
            Assert.NotEmpty(content);
            
            var doc = JsonDocument.Parse(content);
            Assert.True(doc.RootElement.TryGetProperty("data", out _));
        }
    }
}
