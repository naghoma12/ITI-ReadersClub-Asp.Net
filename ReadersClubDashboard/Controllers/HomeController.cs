using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReadersClubCore.Models;
using ReadersClubDashboard.Models;
using ReadersClubDashboard.Service;
using ReadersClubDashboard.Sevice;
using ReadersClubDashboard.ViewModels;
using System.Diagnostics;

namespace ReadersClubDashboard.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly StoryService _storyService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ChannelService _channelService;
        private readonly ReviewService _reviewService;

        public HomeController(ILogger<HomeController> logger
            ,StoryService storyService
            , UserManager<ApplicationUser> userManager
            ,ChannelService channelService,
            ReviewService reviewService)
        {
            _logger = logger;
            _storyService = storyService;
            _userManager = userManager;
            _channelService = channelService;
            _reviewService = reviewService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var statistics = new Statistics();
            var users = await _userManager.GetUsersInRoleAsync("author");
            statistics.AuthorsCount = users.Count();
            if (User.IsInRole("admin"))
            {
                statistics.StoriesCount = await _storyService.GetStoriesCount();
                statistics.ChannelsCount = await _channelService.GetChannelsCount();
                statistics.ReviewsCount = await _reviewService.GetReviewsCount();
         
                var list =  _storyService.GetInValidStories();
                if (list.Any())
                {
                    statistics.NotificationFlag = true;
                }
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                statistics.StoriesCount = await _storyService.GetAuthorStoriesCount(user.Id);
                statistics.ChannelsCount = await _channelService.GetAuthorChannelsCount(user.Id);
                statistics.ReviewsCount = await _reviewService.GetAuthorReviewsCount(user.Id);
            }
            return View(statistics);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
