using Microsoft.AspNetCore.Mvc;
using RodeFortune.BLL.Services.Implementations;
using RodeFortune.DAL.Repositories.Interfaces;

namespace RodeFortune.PresentationLayer.Controllers
{
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
            return View("Index", posts);
        }
    }
}