using Microsoft.EntityFrameworkCore;
using ReadersClubApi.DTO;
using ReadersClubCore.Data;
using ReadersClubCore.Models;

namespace ReadersClubApi.Service
{
    public class ReviewService
    {
        private readonly ReadersClubContext _context;

        public ReviewService(ReadersClubContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReviewDto>> GetAllReviewsInStory(int storyId)
        {
            return await _context.Reviews.
                Include(r => r.Story)
                .Include(r => r.User)
                .Where(r => r.StoryId == storyId
                && r.IsDeleted == false)
                .Select( x=> 
                new ReviewDto
                {
                    Id = x.Id,
                    Comment = x.Comment,
                    Rating = x.Rating,
                    StoryId = x.StoryId,
                    UserId = x.UserId,
                    UserName = x.User.Name,
                    UserImage = x.User.Image,
                    CreatedDate = x.CreatedDate
                }
                )
                .ToListAsync();                ;
                
        }
        public async Task<ReviewDto> GetReviewById(int id)
        {
            return await _context.Reviews
                .Include(r => r.Story)
                .Include(r => r.User)
                .Select(x=>
                new ReviewDto
                {
                    Id = x.Id,
                    Comment = x.Comment,
                    Rating = x.Rating,
                    StoryId = x.StoryId,
                    UserId = x.UserId,
                    UserName = x.User.Name,
                    UserImage = x.User.Image,
                    CreatedDate = x.CreatedDate
                })
                .FirstOrDefaultAsync(r => r.Id == id);
        }
        public async Task AddReview(Review review)
        {
             _context.Reviews.Add(review);
             _context.SaveChanges();
        }
        public async Task UpdateReview(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
        }
    }
}
