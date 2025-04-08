using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using RodeFortune.BLL.Models;
using RodeFortune.DAL.Models;
using RodeFortune.DAL.Repositories.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RodeFortune.BLL.Services.Implementations
{
    public class BloggingService
    {
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<BloggingService> _logger;

        public BloggingService(IPostRepository postRepository, ILogger<BloggingService> logger, IUserRepository userRepository)
        {
            _postRepository = postRepository;
            _logger = logger;
            _userRepository = userRepository;
        }

        //Клас Result в моделях
        public async Task<Result<Post>> CreatePostAsync(ObjectId authorId, string content, string name,
             byte[]? imageUrl = null, ObjectId? Reading = null,
             ObjectId? Horoscope = null, ObjectId? NatalChart = null,
             ObjectId? DestinyMatrix = null)
        {
            var user = await _userRepository.GetByIdAsync(authorId);
            if (user == null)
            {
                _logger.LogWarning($"Failed to create post: User with ID {authorId} not found");
                return new Result<Post>(false, "User was not found", null);
            }

            var dataTime = DateTime.Now;
            try
            {
                var post = new Post
                {
                    Author = authorId,
                    Content = content,
                    CreatedAt = DateTime.SpecifyKind(dataTime, DateTimeKind.Utc),
                    Name = name,
                    ImageData = imageUrl,
                    ReferencedReading = Reading,
                    ReferencedHoroscope = Horoscope,
                    ReferencedNatalChart = NatalChart,
                    ReferencedDestinyMatrix = DestinyMatrix
                };

                await _postRepository.CreateAsync(post);
                _logger.LogInformation($"Created post '{name}' by author {authorId}");
                return new Result<Post>(true, "Post created successfully", post);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating post '{name}' by author {authorId}");
                return new Result<Post>(false, $"Error while creating a post: {ex.Message}", null);
            }
        }


        public async Task<Result<bool>> DeletePostAsync(ObjectId authorId, ObjectId postId)
        {
            try
            {
                var existingPost = await _postRepository.GetByIdAsync(postId);
                if (existingPost == null)
                {
                    _logger.LogWarning($"Post with ID {postId} not found for deletion");
                    return new Result<bool>(false, "Post not found", false);
                }

                if (existingPost.Author != authorId)
                {
                    _logger.LogWarning($"User {authorId} attempted to delete post {postId} owned by {existingPost.Author}");
                    return new Result<bool>(false, "You can only delete your own posts", false);
                }

                await _postRepository.DeleteAsync(postId);
                _logger.LogInformation($"Post {postId} deleted successfully");
                return new Result<bool>(true, "Post deleted successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while deleting post {postId}");
                return new Result<bool>(false, $"Error while deleting post: {ex.Message}", false);
            }
        }
    }
}
