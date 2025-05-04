using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadersClubCore.Data;
using ReadersClubCore.Models;
using ReadersClubDashboard.ViewModels;

namespace ReadersClub.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly ReadersClubContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewsController(ReadersClubContext context
            ,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        static List<ReviewViewModel> reviews = new List<ReviewViewModel>
    {
        new ReviewViewModel
        {
            Id = 1,
            UserName = "نور علي",
            StoryTitle = "العاصفة",
            Comment = "رواية ممتعة وتستحق القراءة",
            Rating = 4
        },
        new ReviewViewModel
        {
            Id = 2,
            UserName = "سارة محمد",
            StoryTitle = "الظلال السوداء",
            Comment = "لم تعجبني النهايات المفتوحة.",
            Rating = 2
        }
            };
        // GET: /Review/
        public async Task<IActionResult> Index()
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Story)
                .Where(r => r.IsDeleted == false)
                .Select(r => new ReviewViewModel
                {
                    Id = r.Id,
                    Comment = r.Comment,
                    Rating = r.Rating,
                    StoryTitle = r.Story.Title,
                    UserName = r.User.Name
                })
                .ToListAsync();

            return View(reviews);
        }
        public async Task<IActionResult> AuthorReviews()
        {
           
            var user = await _userManager.GetUserAsync(User);
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Story)
                .Where(r => r.IsDeleted == false
                && r.UserId == user.Id)
                .Select(r => new ReviewViewModel
                {
                    Id = r.Id,
                    Comment = r.Comment,
                    Rating = r.Rating,
                    StoryTitle = r.Story.Title,
                    UserName = r.User.Name
                })
                .ToListAsync();

            return View("Index",reviews);
        }

        // GET: /Review/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
                return NotFound();

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            if (User.IsInRole("admin"))
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction(nameof(AuthorReviews));
            }
        }
    }
}
