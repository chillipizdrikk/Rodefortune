using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using RodeFortune.BLL.Services.Implementations;
using RodeFortune.DAL.Repositories.Interfaces;
using RodeFortune.PresentationLayer.Models;
using System.Security.Claims;

namespace RodeFortune.PresentationLayer.Controllers;

public class BlogController : Controller
{
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<BlogController> _logger;
    private readonly BloggingService _bloggingService;
    private readonly IReadingRepository _readingRepository;
    private readonly IHoroscopeRepository _horoscopeRepository;
    private readonly INatalChartRepository _natalChartRepository;
    private readonly IDestinyMatrixRepository _destinyMatrixRepository;
    private readonly ICommentRepository _commentRepository;
    public BlogController(
        IPostRepository postRepository,
        IUserRepository userRepository,
        ILogger<BlogController> logger,
        BloggingService bloggingService,
        IReadingRepository readingRepository,
        IHoroscopeRepository horoscopeRepository,
        INatalChartRepository natalChartRepository,
        IDestinyMatrixRepository destinyMatrixRepository,
        ICommentRepository commentRepository)
    {
        _postRepository = postRepository;
        _userRepository = userRepository;
        _logger = logger;
        _bloggingService = bloggingService;
        _readingRepository = readingRepository;
        _horoscopeRepository = horoscopeRepository;
        _natalChartRepository = natalChartRepository;
        _destinyMatrixRepository = destinyMatrixRepository;
        _commentRepository = commentRepository;
    }

    public async Task<IActionResult> Index()
    {
        var posts = await _postRepository.GetAllPostsAsync();
        var postViewModels = posts.Select(pst => new PostViewModel
        {
            Id = pst.Id.ToString(),
            Name = pst.Name,
            Content = pst.Content,
            CreatedAt = pst.CreatedAt,
            Author = pst.Author,
            UpdatedAt = pst.UpdatedAt,
            ImageData = pst.ImageData,
            ReferencedDestinyMatrixId = pst.ReferencedDestinyMatrix?.ToString() ?? string.Empty,
            ReferencedHoroscopeId = pst.ReferencedHoroscope?.ToString() ?? string.Empty,
            ReferencedNatalChartId = pst.ReferencedNatalChart?.ToString() ?? string.Empty,
            ReferencedReadingId = pst.ReferencedReading?.ToString() ?? string.Empty,
        }).ToList();
        return View(postViewModels);
    }

    [HttpGet]
    [Authorize]
    public IActionResult CreatePost()
    {
        return View();
    }
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreatePost(PostViewModel model)
    {

        if (ModelState.IsValid)
        {
            byte[] imageData = null;
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await model.ImageFile.CopyToAsync(memoryStream);
                    imageData = memoryStream.ToArray();
                }
            }

            if (!ObjectId.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out ObjectId authorId))
            {
                return RedirectToAction(nameof(Index));
            }
            var result = await _bloggingService.CreatePostAsync(
                authorId,
                model.Content,
                model.Name,
                imageData,
                string.IsNullOrEmpty(model.ReferencedReadingId) ? null : ObjectId.Parse(model.ReferencedReadingId),
                string.IsNullOrEmpty(model.ReferencedHoroscopeId) ? null : ObjectId.Parse(model.ReferencedHoroscopeId),
                string.IsNullOrEmpty(model.ReferencedNatalChartId) ? null : ObjectId.Parse(model.ReferencedNatalChartId),
                string.IsNullOrEmpty(model.ReferencedDestinyMatrixId) ? null : ObjectId.Parse(model.ReferencedDestinyMatrixId)
            );

            if (result.Success)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]

    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return RedirectToAction("Index");
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Unauthorized();
        }

        var result = await _bloggingService.DeletePostAsync(ObjectId.Parse(currentUserId), ObjectId.Parse(id));

        if (result.Success)
        {
            return RedirectToAction(nameof(Index));
        }
        else
        {
            return RedirectToAction(nameof(Index));
        }
    }



    [HttpGet]
    public async Task<IActionResult> PostDetails(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var post = await _postRepository.GetByIdAsync(ObjectId.Parse(id));

            if (post == null)
            {
                return NotFound();
            }

            var author = await _userRepository.GetByIdAsync(post.Author);
            var comments = await _commentRepository.GetByPostIdAsync(post.Id);

            var viewModel = new PostViewModel
            {
                Id = post.Id.ToString(),
                Name = post.Name,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                Author = post.Author,
                UpdatedAt = post.UpdatedAt,
                ImageData = post.ImageData,
                ReferencedReadingId = post.ReferencedReading?.ToString(),
                ReferencedHoroscopeId = post.ReferencedHoroscope?.ToString(),
                ReferencedNatalChartId = post.ReferencedNatalChart?.ToString(),
                ReferencedDestinyMatrixId = post.ReferencedDestinyMatrix?.ToString()
            };


            var commentsResult = await _bloggingService.GetPostCommentsAsync(post.Id);
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var commentViewModels = new List<CommentViewModel>();
            if (commentsResult.Success && commentsResult.Data != null)
            {
                foreach (var comment in commentsResult.Data)
                {
                    var commentAuthor = await _userRepository.GetByIdAsync(comment.AuthorId);
                    commentViewModels.Add(new CommentViewModel
                    {
                        Id = comment.Id.ToString(),
                        Content = comment.Content,
                        AuthorName = commentAuthor?.Username ?? "Невідомий користувач",
                        CreatedAt = comment.CreatedAt,
                        IsAuthor = currentUserId == comment.AuthorId.ToString(),
                        PostId = post.Id.ToString()
                    });
                }
            }

            ViewBag.AuthorName = author?.Username ?? "Невідомий автор";
            ViewBag.Comments = commentViewModels;
            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting post details for ID: {PostId}", id);
            return RedirectToAction(nameof(Index));
        }
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> AddComment(string postId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            TempData["Error"] = "Коментар не може бути порожнім";
            return RedirectToAction(nameof(PostDetails), new { id = postId });
        }

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var result = await _bloggingService.AddCommentAsync(
                ObjectId.Parse(postId),
                ObjectId.Parse(userId),
                content);

            if (result.Success)
            {
                TempData["Success"] = "Коментар додано успішно";
            }
            else
            {
                TempData["Error"] = result.Message;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding comment");
            TempData["Error"] = "Помилка при додаванні коментаря";
        }

        return RedirectToAction(nameof(PostDetails), new { id = postId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> DeleteComment(string commentId, string postId)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _bloggingService.DeleteCommentAsync(
                ObjectId.Parse(commentId),
                ObjectId.Parse(userId));

            if (result.Success)
            {
                TempData["Success"] = "Коментар видалено успішно";
            }
            else
            {
                TempData["Error"] = result.Message;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting comment");
            TempData["Error"] = "Помилка при видаленні коментаря";
        }

        return RedirectToAction(nameof(PostDetails), new { id = postId });
    }
}