using Microsoft.EntityFrameworkCore;
using ReadersClubCore.Data;

namespace ReadersClubDashboard.Sevice
{
    public class ReviewService
    {
        private readonly ReadersClubContext _context;

        public ReviewService(ReadersClubContext context)
        {
            _context = context;
        }

        public async Task<int> GetReviewsCount()
        {
            return await _context.Reviews
                .Where(x => x.IsDeleted == false)
                .CountAsync();
        }
        public async Task<int> GetAuthorReviewsCount(int userId)
        {
            return await _context.Reviews
                .Where(x => x.IsDeleted == false
                && x.UserId == userId)
                .CountAsync();
        }
    }
}
