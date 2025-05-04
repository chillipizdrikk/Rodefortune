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
        private Mock<ICommentRepository> _mockCommentRepository;
        private Mock<ILogger<BloggingService>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockPostRepository = new Mock<IPostRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILogger<BloggingService>>();
            _mockCommentRepository = new Mock<ICommentRepository>();
            _bloggingService = new BloggingService(
                _mockPostRepository.Object,
                _mockLogger.Object,
                _mockUserRepository.Object,
                _mockCommentRepository.Object
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


        [Test]
        public async Task AddCommentAsync_ShouldReturnError_WhenContentIsEmpty()
        {
            var postId = ObjectId.GenerateNewId();
            var userId = ObjectId.GenerateNewId();
            string content = "";

            var result = await _bloggingService.AddCommentAsync(postId, userId, content);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Data, Is.Null);
            Assert.That(result.Message, Is.EqualTo("Content cannot be empty"));
        }
        [Test]
        public async Task AddCommentAsync_ShouldReturnError_WhenUserNotFound()
        {
            var postId = ObjectId.GenerateNewId();
            var userId = ObjectId.GenerateNewId();
            string content = "Test comment";

            var post = new Post { Id = postId };
            _mockPostRepository.Setup(r => r.GetByIdAsync(postId)).ReturnsAsync(post);
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User)null);

            var result = await _bloggingService.AddCommentAsync(postId, userId, content);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Data, Is.Null);
            Assert.That(result.Message, Is.EqualTo("User not found"));
        }

        [Test]
        public async Task AddCommentAsync_ShouldReturnSuccess_WhenValidDataProvided()
        {
            var postId = ObjectId.GenerateNewId();
            var userId = ObjectId.GenerateNewId();
            string content = "Test comment";

            var post = new Post { Id = postId, Comments = new List<ObjectId>() };
            var user = new User { Id = userId };

            _mockPostRepository.Setup(r => r.GetByIdAsync(postId)).ReturnsAsync(post);
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            Comment capturedComment = null;
            _mockCommentRepository.Setup(r => r.CreateAsync(It.IsAny<Comment>()))
                .Callback<Comment>(comment => capturedComment = comment)
                .Returns(Task.CompletedTask);

            var result = await _bloggingService.AddCommentAsync(postId, userId, content);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Message, Is.EqualTo("Comment created successfully"));

            Assert.That(capturedComment, Is.Not.Null);
            Assert.That(capturedComment.PostId, Is.EqualTo(postId));
            Assert.That(capturedComment.AuthorId, Is.EqualTo(userId));
            Assert.That(capturedComment.Content, Is.EqualTo(content));
            Assert.That(capturedComment.CreatedAt.Date, Is.EqualTo(DateTime.UtcNow.Date));

            _mockPostRepository.Verify(r => r.UpdateAsync(post), Times.Once);
            Assert.That(post.Comments.Contains(capturedComment.Id), Is.True);
        }

        [Test]
        public async Task GetPostCommentsAsync_ShouldReturnSuccess_WhenCommentsExist()
        {
            var postId = ObjectId.GenerateNewId();
            var comments = new List<Comment>
            {
                new Comment { Id = ObjectId.GenerateNewId(), PostId = postId },
                new Comment { Id = ObjectId.GenerateNewId(), PostId = postId }
            };

            _mockCommentRepository.Setup(r => r.GetByPostIdAsync(postId)).ReturnsAsync(comments);

            var result = await _bloggingService.GetPostCommentsAsync(postId);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Count, Is.EqualTo(2));
            Assert.That(result.Message, Is.EqualTo("Comments retrieved successfully"));
        }

        [Test]
        public async Task GetPostCommentsAsync_ShouldReturnEmptyList_WhenNoComments()
        {
            var postId = ObjectId.GenerateNewId();

            _mockCommentRepository.Setup(r => r.GetByPostIdAsync(postId))
                .ReturnsAsync(new List<Comment>());

            var result = await _bloggingService.GetPostCommentsAsync(postId);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Count, Is.EqualTo(0));
            Assert.That(result.Message, Is.EqualTo("Comments retrieved successfully"));
        }

        [Test]
        public async Task DeleteCommentAsync_ShouldReturnError_WhenCommentNotFound()
        {
            var commentId = ObjectId.GenerateNewId();
            var userId = ObjectId.GenerateNewId();

            _mockCommentRepository.Setup(r => r.GetByIdAsync(commentId)).ReturnsAsync((Comment)null);

            var result = await _bloggingService.DeleteCommentAsync(commentId, userId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Comment not found"));
        }

        [Test]
        public async Task DeleteCommentAsync_ShouldReturnError_WhenUserNotAuthor()
        {
            var commentId = ObjectId.GenerateNewId();
            var userId = ObjectId.GenerateNewId();
            var authorId = ObjectId.GenerateNewId();

            var comment = new Comment { Id = commentId, AuthorId = authorId };
            _mockCommentRepository.Setup(r => r.GetByIdAsync(commentId)).ReturnsAsync(comment);

            var result = await _bloggingService.DeleteCommentAsync(commentId, userId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("You can only delete your own comments"));
        }

        [Test]
        public async Task DeleteCommentAsync_ShouldReturnSuccess_WhenUserIsAuthor()
        {
            var commentId = ObjectId.GenerateNewId();
            var userId = ObjectId.GenerateNewId();

            var comment = new Comment { Id = commentId, AuthorId = userId };
            _mockCommentRepository.Setup(r => r.GetByIdAsync(commentId)).ReturnsAsync(comment);
            _mockCommentRepository.Setup(r => r.DeleteAsync(commentId)).ReturnsAsync(true);

            var result = await _bloggingService.DeleteCommentAsync(commentId, userId);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.True);
            Assert.That(result.Message, Is.EqualTo("Comment deleted successfully"));
            _mockCommentRepository.Verify(r => r.DeleteAsync(commentId), Times.Once);
        }

        [Test]
        public async Task DeleteCommentAsync_ShouldReturnError_WhenDeletionFails()
        {
            var commentId = ObjectId.GenerateNewId();
            var userId = ObjectId.GenerateNewId();

            var comment = new Comment { Id = commentId, AuthorId = userId };
            _mockCommentRepository.Setup(r => r.GetByIdAsync(commentId)).ReturnsAsync(comment);
            _mockCommentRepository.Setup(r => r.DeleteAsync(commentId)).ReturnsAsync(false);

            var result = await _bloggingService.DeleteCommentAsync(commentId, userId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Failed to delete comment"));
        }


        [Test]
        public async Task UpdateCommentAsync_ShouldReturnError_WhenCommentNotFound()
        {
            var commentId = ObjectId.GenerateNewId();
            var userId = ObjectId.GenerateNewId();

            _mockCommentRepository
                .Setup(repo => repo.GetByIdAsync(commentId))
                .ReturnsAsync((Comment)null!);

            var result = await _bloggingService.UpdateCommentAsync(commentId, userId, "New content");

            Assert.That(result.Success, Is.False);
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Comment not found"));
        }

        [Test]
        public async Task UpdateCommentAsync_ShouldReturnError_WhenUserNotAuthor()
        {
            var commentId = ObjectId.GenerateNewId();
            var authorId = ObjectId.GenerateNewId();
            var userId = ObjectId.GenerateNewId();

            var comment = new Comment { Id = commentId, AuthorId = authorId };

            _mockCommentRepository
                .Setup(repo => repo.GetByIdAsync(commentId))
                .ReturnsAsync(comment);

            var result = await _bloggingService.UpdateCommentAsync(commentId, userId, "New content");

            Assert.That(result.Success, Is.False);
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("You can only update your own comments"));
        }

        [Test]
        public async Task UpdateCommentAsync_ShouldReturnSuccess_WhenUpdateSucceeds()
        {
            var commentId = ObjectId.GenerateNewId();
            var userId = ObjectId.GenerateNewId();

            var comment = new Comment { Id = commentId, AuthorId = userId, Content = "Old" };

            _mockCommentRepository
                .Setup(repo => repo.GetByIdAsync(commentId))
                .ReturnsAsync(comment);

            _mockCommentRepository
                .Setup(repo => repo.UpdateAsync(It.IsAny<Comment>()))
                .ReturnsAsync(true);

            var result = await _bloggingService.UpdateCommentAsync(commentId, userId, "New");

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.True);
            Assert.That(result.Message, Is.EqualTo("Comment updated successfully"));
            _mockCommentRepository.Verify(r => r.UpdateAsync(It.Is<Comment>(c => c.Content == "New")), Times.Once);
        }

        [Test]
        public async Task UpdateCommentAsync_ShouldReturnError_WhenUpdateFails()
        {
            var commentId = ObjectId.GenerateNewId();
            var userId = ObjectId.GenerateNewId();

            var comment = new Comment { Id = commentId, AuthorId = userId };

            _mockCommentRepository
                .Setup(repo => repo.GetByIdAsync(commentId))
                .ReturnsAsync(comment);

            _mockCommentRepository
                .Setup(repo => repo.UpdateAsync(It.IsAny<Comment>()))
                .ReturnsAsync(false);

            var result = await _bloggingService.UpdateCommentAsync(commentId, userId, "Updated");

            Assert.That(result.Success, Is.False);
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Failed to update comment"));
        }

    }
}