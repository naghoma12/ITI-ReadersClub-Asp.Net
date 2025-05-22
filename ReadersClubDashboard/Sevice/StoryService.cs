using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadersClubCore.Data;
using ReadersClubCore.Models;
using ReadersClubDashboard.ViewModels;

namespace ReadersClubDashboard.Service
{
    public class StoryService
    {
        private readonly ReadersClubContext _context;
        public StoryService([FromServices] ReadersClubContext context)
        {
            _context = context;
        }
        #region Methods For Both
        //Details for admin & author
        public Story GetStoryById(int id)
        {
            return _context
                .Stories.
                Where(x => x.IsDeleted == false)
                .Include(c => c.Category)
                .Include(c => c.Channel)
                .Include(u => u.User)
                .FirstOrDefault(s => s.Id == id);
        }

        //Add story for admin & author
        public void AddStory(Story story)
        {
            _context.Stories.Add(story);
            _context.SaveChanges();
        }

        public void UpdateStory(Story story)
        {
            _context.Stories.Update(story);
            _context.SaveChanges();
        }
        public void DeleteStory(Story story)
        {
            _context.Stories.Remove(story);
            _context.SaveChanges();

        }
        #endregion

        #region Methods For Admin
        public List<StoryVM> GetAllStories()
        {
            return _context.Stories
                .Include(c => c.Category)
                .Include(c => c.Channel)
                .Where(x => x.IsDeleted == false
                        && x.IsValid == true
                        && x.IsActive == true
                        && x.Status == Status.Approved)
                 .Select(x => new StoryVM
                 {
                     Story = x,
                     AverageRating = x.Reviews.Average(r => r.Rating)
                 })
                .ToList();
        }
       //Is not valid Stories
       public List<StoryVM> GetInValidStories()
        {
            return _context.Stories
               .Include(c => c.Category)
                .Include(c => c.Channel)
                .Where(x => x.IsDeleted == false
                        && x.IsValid == false
                        && x.IsActive == true
                        && x.Status == Status.Pending)
                 .Select(x => new StoryVM
                 {
                     Story = x,
                     AverageRating = x.Reviews.Average(r => r.Rating)
                 })
                .ToList();
        }

        public List<Story> GetInActiveStories()
        {
            return _context.Stories
                .Where(x => x.IsDeleted == false
                && x.IsActive == false
                && x.IsValid == true)
                .ToList();
        }
        // القصص الأكثر مشاهده للأدمن
        public async Task<IEnumerable<StoryVM>> MostViewedStories()
        {
            var stories = await _context.Stories
                .Where(x => x.IsDeleted == false
                && x.IsValid == true
                && x.IsActive == true
                && x.Status == Status.Approved)
                .Include(c => c.Category)
                .Select(x => new StoryVM
                {
                    Story = x,
                    AverageRating = x.Reviews.Average(r => r.Rating)
                })
                .OrderByDescending(x => x.Story.ViewsCount)
                .Take(15)
                .ToListAsync();
            return stories;
        }

        // القصص الأكثر تقييما للأدمن
        public async Task<IEnumerable<StoryVM>> MostRatedStories()
        {
            var stories = await _context.Stories
                .Where(x => x.IsValid == true
                && x.IsDeleted == false
                && x.IsActive == true
                && x.Status == Status.Approved)
                .Include(c => c.Category)
                .Select(x => new StoryVM
                {
                    Story = x,
                    AverageRating = x.Reviews.Average(r => r.Rating)
                })
                .OrderByDescending(x => x.AverageRating)
                .Take(15)
                .ToListAsync();
            return stories;

        }
        public Task<int> GetStoriesCount()
        {
            return _context.Stories
                .Where(x => x.IsDeleted == false
                && x.IsValid == true
                && x.Status == Status.Approved)
                .CountAsync();
        }
        public List<Story> GetStoriesByCategory(int categoryId)
        {
            return _context.Stories.Where(s => s.CategoryId == categoryId).ToList();
        }
        public List<Story> GetStoriesByUser(int userId)
        {
            return _context.Stories.Where(s => s.UserId == userId).ToList();
        }
        public List<Story> GetStoriesByChannel(int channelId)
        {
            return _context.Stories.Where(s => s.ChannelId == channelId).ToList();
        }
        public List<Story> GetStoriesByStatus(Status status)
        {
            return _context.Stories.Where(s => s.Status == status).ToList();
        }
        public List<Story> GetStoriesByTitle(string title)
        {
            return _context.Stories.Where(s => s.Title.Contains(title)).ToList();
        }
        #endregion

        #region Methods For Author

        public List<StoryVM> GetAllStories(int userId)
        {
            return _context.Stories
                .Include(c => c.Category)
                .Include(c => c.Channel)
                .Where(x => x.UserId == userId 
                && x.IsDeleted == false 
                && x.IsValid == true
                && x.Status == Status.Approved
                && x.IsActive == true)
                 .Select(x => new StoryVM
                 {
                     Story = x,
                     AverageRating = x.Reviews.Average(r => r.Rating)
                 })
                .ToList();
        }
        // القصص الأكثر مشاهده للكاتب
        public async Task<IEnumerable<StoryVM>> MostViewedStories(int userId)
        {
            var stories = await _context.Stories
                .Where(x => x.IsDeleted == false
                && x.IsValid == true
                && x.UserId == userId
                && x.IsActive == true)
                .Include(c => c.Category)
                 .Select(x => new StoryVM
                 {
                     Story = x,
                     AverageRating = x.Reviews.Average(r => r.Rating)
                 })
                .OrderByDescending(x => x.Story.ViewsCount)
                .Take(15)
                .ToListAsync();
            return stories;

        }
        public Task<int> GetAuthorStoriesCount(int id)
        {
            return _context.Stories
                .Where(x => x.IsDeleted == false
                && x.IsValid == true
                && x.Status == Status.Approved
                && x.UserId == id)
                .CountAsync();
        }
        // القصص الأكثر تقييما للكاتب
        public async Task<IEnumerable<StoryVM>> MostRatedStories(int userId)
        {
            var stories = await _context.Stories
                .Where(x => x.IsValid == true
                && x.IsDeleted == false
                && x.UserId == userId
                && x.IsActive == true)
                .Include(c => c.Category)
                .Select(x => new StoryVM
                {
                    Story = x,
                    AverageRating = x.Reviews.Average(r => r.Rating)
                })
                .OrderByDescending(x => x.AverageRating)
                .Take(15)
                .ToListAsync();
            return stories;

        }
        #endregion

        public List<Story> GetStoriesByIsActive(bool isActive)
        {

            return _context.Stories.Where(s => s.IsActive == isActive).ToList();
        }
        public List<Story> GetStoriesByIsValid(bool isValid)
        {
            return _context.Stories.Where(s => s.IsValid == isValid).ToList();
        }
        public List<Story> GetStoriesByDescription(string description)
        {
            return _context.Stories.Where(s => s.Description.Contains(description)).ToList();

        }




    } 
}
