using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using RodeFortune.BLL.Services.Implementations;
using RodeFortune.DAL.Models;
using RodeFortune.DAL.Repositories.Interfaces;

namespace RodeFortune.UnitTests.Services
{
    [TestFixture]
    public class BloggingServiceTest
    {
        private BloggingService _bloggingService;
        private Mock<IPostRepository> _mockPostRepository;
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<ILogger<BloggingService>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockPostRepository = new Mock<IPostRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILogger<BloggingService>>();
            _bloggingService = new BloggingService(
                _mockPostRepository.Object,
                _mockLogger.Object,
                _mockUserRepository.Object
            );
        }


        [Test]
        public async Task CreatePostAsync_ShouldReturnError_WhenUserNotFound()
        {
            var userId = ObjectId.GenerateNewId();
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User)null);

            var result = await _bloggingService.CreatePostAsync(userId, "Test content", "Test Post");

            Assert.That(result.Success, Is.False);
            Assert.That(result.Data, Is.Null);
            Assert.That(result.Message, Is.EqualTo("User was not found"));
        }

        [Test]
        public async Task CreatePostAsync_ShouldReturnSuccess_WhenValidDataProvided()
        {
            var userId = ObjectId.GenerateNewId();
            var user = new User { Id = userId };
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            Post capturedPost = null;
            _mockPostRepository.Setup(r => r.CreateAsync(It.IsAny<Post>()))
                .Callback<Post>(post => capturedPost = post)
                .Returns(Task.CompletedTask);

            var result = await _bloggingService.CreatePostAsync(
                userId,
                "Test content",
                "Test Post",
                new byte[] { 1, 2, 3 },
                ObjectId.GenerateNewId(),
                ObjectId.GenerateNewId(),
                ObjectId.GenerateNewId(),
                ObjectId.GenerateNewId()
            );

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Message, Is.EqualTo("Post created successfully"));

            Assert.That(capturedPost, Is.Not.Null);
            Assert.That(capturedPost.Author, Is.EqualTo(userId));
            Assert.That(capturedPost.Content, Is.EqualTo("Test content"));
            Assert.That(capturedPost.Name, Is.EqualTo("Test Post"));
            Assert.That(capturedPost.ImageData, Is.Not.Null);
            Assert.That(capturedPost.ReferencedReading, Is.Not.Null);
            Assert.That(capturedPost.ReferencedHoroscope, Is.Not.Null);
            Assert.That(capturedPost.ReferencedNatalChart, Is.Not.Null);
            Assert.That(capturedPost.ReferencedDestinyMatrix, Is.Not.Null);
        }


        [Test]
        public async Task DeletePostAsync_ShouldReturnError_WhenPostNotFound()
        {
            var userId = ObjectId.GenerateNewId();
            var postId = ObjectId.GenerateNewId();
            _mockPostRepository.Setup(r => r.GetByIdAsync(postId)).ReturnsAsync((Post)null);

            var result = await _bloggingService.DeletePostAsync(userId, postId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Post not found"));
        }

        [Test]
        public async Task DeletePostAsync_ShouldReturnError_WhenUserNotAuthor()
        {
            var userId = ObjectId.GenerateNewId();
            var authorId = ObjectId.GenerateNewId();
            var postId = ObjectId.GenerateNewId();

            var post = new Post { Id = postId, Author = authorId };
            _mockPostRepository.Setup(r => r.GetByIdAsync(postId)).ReturnsAsync(post);

            var result = await _bloggingService.DeletePostAsync(userId, postId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("You can only delete your own posts"));
        }

        [Test]
        public async Task DeletePostAsync_ShouldReturnSuccess_WhenUserIsAuthor()
        {
            var userId = ObjectId.GenerateNewId();
            var postId = ObjectId.GenerateNewId();

            var post = new Post { Id = postId, Author = userId };
            _mockPostRepository.Setup(r => r.GetByIdAsync(postId)).ReturnsAsync(post);

            _mockPostRepository.Setup(r => r.DeleteAsync(postId)).ReturnsAsync(true);

            var result = await _bloggingService.DeletePostAsync(userId, postId);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.True);
            Assert.That(result.Message, Is.EqualTo("Post deleted successfully"));
            _mockPostRepository.Verify(r => r.DeleteAsync(postId), Times.Once);
        }

      
    }
}