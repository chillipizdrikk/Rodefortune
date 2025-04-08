using Microsoft.AspNetCore.Mvc;
using RodeFortune.BLL.Services.Implementations;
using RodeFortune.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using RodeFortune.PresentationLayer.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Bson;
using System.Security.Claims;
using RodeFortune.DAL.Models;

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

    public BlogController(
        IPostRepository postRepository,
        IUserRepository userRepository,
        ILogger<BlogController> logger,
        BloggingService bloggingService,
        IReadingRepository readingRepository,
    IHoroscopeRepository horoscopeRepository,
        INatalChartRepository natalChartRepository,
        IDestinyMatrixRepository destinyMatrixRepository)
    {
        _postRepository = postRepository;
        _userRepository = userRepository;
        _logger = logger;
        _bloggingService = bloggingService;
        _readingRepository = readingRepository;
        _horoscopeRepository = horoscopeRepository;
        _natalChartRepository = natalChartRepository;
        _destinyMatrixRepository = destinyMatrixRepository;
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
}