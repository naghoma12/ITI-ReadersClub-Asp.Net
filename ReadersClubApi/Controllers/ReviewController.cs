using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReadersClubApi.DTO;
using ReadersClubApi.Service;
using ReadersClubCore.Models;

namespace ReadersClubApi.Controllers
{
    
    public class ReviewController : BaseController
    {
        private readonly ReviewService _reviewService;

        public ReviewController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost("AddReview")]
        public async Task<IActionResult> AddReview(ReviewDto reviewdto)
        {
            if (ModelState.IsValid)
            {
                var review = new Review()
                {
                    Comment = reviewdto.Comment,
                    Rating = reviewdto.Rating,
                    StoryId = reviewdto.StoryId,
                    UserId = reviewdto.UserId
                };
                await _reviewService.AddReview(review);
                return Ok("Review added successfully");
            }
            return BadRequest("Invalid review data");
        }
        [HttpPut("UpdateReview")]
        [Authorize]
        public async Task<IActionResult> UpdateReview(int id, ReviewDto reviewdto)
        {
            if (ModelState.IsValid)
            {
                var existingReview = await _reviewService.GetReviewById(id);
                if (existingReview == null)
                {
                    return NotFound("Review not found");
                }
                existingReview.Comment = reviewdto.Comment;
                existingReview.Rating = reviewdto.Rating;
                existingReview.UserId = reviewdto.UserId;
                existingReview.StoryId = reviewdto.StoryId;
                var review = new Review()
                {
                    Comment = existingReview.Comment,
                    Rating = existingReview.Rating,
                    StoryId = existingReview.StoryId,
                    UserId = existingReview.UserId
                };
                await _reviewService.UpdateReview(review);
                return Ok("Review updated successfully");

            }
            return BadRequest("Invalid review data");
        }
        [HttpDelete("DeleteReview")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var existingReview = await _reviewService.GetReviewById(id);
            if (existingReview == null)
            {
                return NotFound("Review not found");
            }
            var review = new Review()
            {
                Comment = existingReview.Comment,
                Rating = existingReview.Rating,
                StoryId = existingReview.StoryId,
                UserId = existingReview.UserId
            };
            review.IsDeleted = true;
            await _reviewService.UpdateReview(review);
            return Ok("Review deleted successfully");
        }
    }
}
