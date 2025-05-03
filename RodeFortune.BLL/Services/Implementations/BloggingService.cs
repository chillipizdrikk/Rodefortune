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
        private readonly ICommentRepository _commentRepository;
        private readonly ILogger<BloggingService> _logger;

        public BloggingService(IPostRepository postRepository, ILogger<BloggingService> logger, IUserRepository userRepository,
            ICommentRepository commentRepository)
        {
            _postRepository = postRepository;
            _logger = logger;
            _userRepository = userRepository;
            _commentRepository = commentRepository;
        }

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

                if (existingPost.Comments != null && existingPost.Comments.Any())
                {
                    foreach (var commentId in existingPost.Comments)
                    {
                        await _commentRepository.DeleteAsync(commentId);
                    }
                    _logger.LogInformation($"Deleted {existingPost.Comments.Count} comments associated with post {postId}");
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

        public async Task<Result<Comment>> AddCommentAsync(ObjectId postId, ObjectId authorId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                _logger.LogWarning($"Failed to add comment: Content is empty");
                return new Result<Comment>(false, "Content cannot be empty", null);
            }
            try
            {
                var post = await _postRepository.GetByIdAsync(postId);
                if (post == null)
                {
                    _logger.LogWarning($"Failed to add comment: Post with ID {postId} not found");
                    return new Result<Comment>(false, "Post not found", null);
                }

                var user = await _userRepository.GetByIdAsync(authorId);
                if (user == null)
                {
                    _logger.LogWarning($"Failed to add comment: User with ID {authorId} not found");
                    return new Result<Comment>(false, "User not found", null);
                }

                var comment = new Comment
                {
                    PostId = postId,
                    AuthorId = authorId,
                    Content = content,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _commentRepository.CreateAsync(comment);

                post.Comments.Add(comment.Id);
                await _postRepository.UpdateAsync(post);

                _logger.LogInformation($"Created comment by author {authorId} on post {postId} and updated post");

                return new Result<Comment>(true, "Comment created successfully", comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating comment by author {authorId} on post {postId}");
                return new Result<Comment>(false, $"Error while creating comment: {ex.Message}", null);
            }
        }

        public async Task<Result<List<Comment>>> GetPostCommentsAsync(ObjectId postId)
        {
            try
            {
                var comments = await _commentRepository.GetByPostIdAsync(postId);
                return new Result<List<Comment>>(true, "Comments retrieved successfully", comments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving comments for post {postId}");
                return new Result<List<Comment>>(false, $"Error while retrieving comments: {ex.Message}", null);
            }
        }

        public async Task<Result<bool>> DeleteCommentAsync(ObjectId commentId, ObjectId userId)
        {
            try
            {
                var comment = await _commentRepository.GetByIdAsync(commentId);
                if (comment == null)
                {
                    _logger.LogWarning($"Comment with ID {commentId} not found for deletion");
                    return new Result<bool>(false, "Comment not found", false);
                }

                if (comment.AuthorId != userId)
                {
                    _logger.LogWarning($"User {userId} attempted to delete comment {commentId} owned by {comment.AuthorId}");
                    return new Result<bool>(false, "You can only delete your own comments", false);
                }

                var deleted = await _commentRepository.DeleteAsync(commentId);
                if (deleted)
                {
                    _logger.LogInformation($"Comment {commentId} deleted successfully");
                    return new Result<bool>(true, "Comment deleted successfully", true);
                }
                else
                {
                    return new Result<bool>(false, "Failed to delete comment", false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while deleting comment {commentId}");
                return new Result<bool>(false, $"Error while deleting comment: {ex.Message}", false);
            }
        }

        public async Task<Result<bool>> UpdateCommentAsync(ObjectId commentId, ObjectId userId, string content)
        {
            try
            {
                var comment = await _commentRepository.GetByIdAsync(commentId);
                if (comment == null)
                {
                    _logger.LogWarning($"Comment with ID {commentId} not found for update");
                    return new Result<bool>(false, "Comment not found", false);
                }

                if (comment.AuthorId != userId)
                {
                    _logger.LogWarning($"User {userId} attempted to update comment {commentId} owned by {comment.AuthorId}");
                    return new Result<bool>(false, "You can only update your own comments", false);
                }

                comment.Content = content;
                var updated = await _commentRepository.UpdateAsync(comment);

                if (updated)
                {
                    _logger.LogInformation($"Comment {commentId} updated successfully");
                    return new Result<bool>(true, "Comment updated successfully", true);
                }
                else
                {
                    return new Result<bool>(false, "Failed to update comment", false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while updating comment {commentId}");
                return new Result<bool>(false, $"Error while updating comment: {ex.Message}", false);
            }
        }
    }
}

