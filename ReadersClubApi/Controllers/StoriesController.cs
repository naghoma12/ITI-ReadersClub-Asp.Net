using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadersClubApi.DTO;
using ReadersClubApi.Service;
using ReadersClubApi.Services;
using ReadersClubCore.Models;
using System.Net;
using System.Security.Claims;

namespace ReadersClubApi.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class StoriesController : ControllerBase
    {
        private readonly StoryService _storyService;
        private readonly ReviewService _reviewService;
        private readonly UserManager<ApplicationUser> _userManager;

        public StoriesController(StoryService storyService
            ,ReviewService reviewService,
            UserManager<ApplicationUser> userManager)
        {
            _storyService = storyService;
            _reviewService = reviewService;
            _userManager = userManager;
        }

        [HttpGet("popular")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetPopularStories()
        {
            var stories = _storyService.GetMostPopularStories();
            return Ok(stories);
        }
        [HttpGet("MostViewed")]
        public async Task<IActionResult> GetMostViewedStories()
        {
            var stories = await _storyService.MostViewedStories();
            return Ok(stories);
        }

        [HttpGet]
        public IActionResult GetAllStories()
            {
            var stories = _storyService.GetAllStories();
            return Ok(stories);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStoryById(int id)
        {
            var story = _storyService.GetStoryById(id);
            var storyReviews = await _reviewService.GetAllReviewsInStory(story.Id);
            if (story == null)
                return NotFound();
            story.Reviews = storyReviews;
            return Ok(story);

        }
     
        [HttpPost("{id}/increase-views")]
        public IActionResult IncreaseViews(int id)
        {
             _storyService.UpdateStoryViewsCount(id);
           
            return Ok();
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("{id}/like")]
        public IActionResult LikeStory(int id)
        {
            _storyService.UpdateStoryLikesCount(id);
            return Ok();
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("{id}/dislike")]
        public IActionResult DislikeStory(int id)
        {
            _storyService.UpdateStoryDislikesCount(id);
            return Ok();
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("{id}/unlike")]
        public IActionResult UnlikeStory(int id)
        {
            _storyService.UpdateStoryUnlikesCount(id);
            return Ok();
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("{id}/undislike")]
        public IActionResult UndislikeStory(int id)
        {
            _storyService.UpdateStoryUnDislikesCount(id);
            return Ok();
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("{storyId}/toggle-save")]
        public IActionResult ToggleSaveStory(int storyId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var isSaved = _storyService.ToggleSaveStory(storyId, userId);
            return Ok(new { isSaved });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{storyId}/issaved")]
        public IActionResult IsStorySaved(int storyId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var result = _storyService.IsStorySaved(storyId, userId);
            return Ok(result);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("saved")]
        public async Task<IActionResult> GetSavedStories()
        {
            // استخراج userId من التوكن
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = Convert.ToInt32(userIdClaim);

            // استدعاء الدالة في الخدمة
            var savedStories = await _storyService.GetSavedStoriesByUserId(userId);

            return Ok(savedStories);
        }
        [HttpGet("FilterStory")]
       public async Task<IActionResult> FilterStory([FromQuery]string? title)
        {
            var stories = await _storyService.FilterStory(title);
            return Ok(stories);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("add-review")]
        public async Task<IActionResult> AddReview([FromBody] ReviewDto reviewDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            reviewDto.UserId = userId;
            var review = new Review
            {
                Comment = reviewDto.Comment,
                Rating = reviewDto.Rating,
                StoryId = reviewDto.StoryId,
                UserId = userId
            };
            await _reviewService.AddReview(review);
            return Ok();
        }
        [HttpPost("SetLastPage")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> SetStoryPage(int storyId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            try
            {
                await _storyService.AddStoryLastPage(userId, storyId);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest("ex.message");
            }
        }




    }

}