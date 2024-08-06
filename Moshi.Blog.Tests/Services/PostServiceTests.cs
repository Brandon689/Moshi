using Moq;
using Moshi.Blog.Data;
using Moshi.Blog.Models;
using Moshi.Blog.Services;

namespace Moshi.Blog.Tests.UnitTests.Services
{
    public class PostServiceTests
    {
        [Fact]
        public async Task GetPostById_ReturnsPost_WhenPostExists()
        {
            // Arrange
            var mockRepo = new Mock<PostRepository>(null);
            var expectedPost = new Post { Id = 1, Title = "Test Post" };
            mockRepo.Setup(repo => repo.GetPostById(1)).ReturnsAsync(expectedPost);
            var service = new PostService(mockRepo.Object);

            // Act
            var result = await service.GetPostById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedPost.Id, result.Id);
            Assert.Equal(expectedPost.Title, result.Title);
        }
    }
}
