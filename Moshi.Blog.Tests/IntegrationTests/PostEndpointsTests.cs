using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moshi.Blog.Models;
using Moshi.Blog.Tests.Helpers;
using System.Net;
using System.Net.Http.Json;

namespace Moshi.Blog.Tests.IntegrationTests
{
    public class PostEndpointsTests : IClassFixture<TestWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public PostEndpointsTests(TestWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllPosts_ReturnsSuccessStatusCode()
        {
            // Arrange
            var response = await _client.GetAsync("/api/posts");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreatePost_ReturnsCreatedPost()
        {
            // Arrange
            var newPost = new Post { Title = "Test Post", Content = "This is a test post." };

            // Act
            var response = await _client.PostAsJsonAsync("/api/posts", newPost);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var returnedPost = await response.Content.ReadFromJsonAsync<Post>();
            Assert.NotNull(returnedPost);
            Assert.Equal(newPost.Title, returnedPost.Title);
            Assert.Equal(newPost.Content, returnedPost.Content);
        }
    }
}
