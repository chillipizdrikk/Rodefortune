using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using RodeFortune.BLL.Models;
using RodeFortune.DAL.Models;
using RodeFortune.DAL.Repositories.Interfaces;

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

            try
            {
                var post = new Post
                {
                    Author = authorId,
                    Content = content,
                    CreatedAt = DateTime.UtcNow,
                    Name = name,
                    ImageUrl = imageUrl,
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
    }
}
