using Microsoft.EntityFrameworkCore;
using ReadersClubApi.DTO;
using ReadersClubCore.Data;
using ReadersClubCore.Models;
using System.Linq;

namespace ReadersClubApi.Services
{
    public class StoryService
    {
        private readonly ReadersClubContext _context;

        public StoryService(ReadersClubContext context)
        {
            _context = context;
        }

        public List<PopularStoryDTO> GetMostPopularStories()
        {
            var stories = _context.Stories
                .Include(s => s.Channel)
                .Include(s => s.Category)
                .Where(s => s.IsValid && !s.IsDeleted
                && s.Status == Status.Approved)
                .Select(s => new PopularStoryDTO
                {
                    Id = s.Id,
                    Title = s.Title,
                    Cover = $"http://readersclub.runasp.net//Uploads/Covers/{s.Cover}",
                    AverageRating = s.Reviews.Any() ? s.Reviews.Average(r => (float)r.Rating) : 0,
                    ChannelName = s.Channel.Name,
                    ChannelImage = string.IsNullOrEmpty(s.Channel.Image) ?
                    $"http://readersclub.runasp.net//Uploads/ChannelsImages/welcome-image.jpeg" :
                    $"http://readersclub.runasp.net//Uploads/ChannelsImages/{s.Channel.Image}",
                    CategoryName = s.Category.Name
                })
                .OrderByDescending(s => s.AverageRating)
                .Take(8)
                .ToList();

            return stories;
        }
        public List<StoryDto> GetAllStories()
        {
            var allstories = _context.Stories
                .Include(s => s.Channel)
                .Include(s => s.Category)
                .Where(s => s.IsValid && !s.IsDeleted && s.IsActive && s.Status == Status.Approved)
                .Select(s => new StoryDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    Cover = $"http://readersclub.runasp.net//Uploads/Covers/{s.Cover}",
                    Description = s.Description,
                    Summary = s.Summary,
                    AverageRating = s.Reviews.Any() ? s.Reviews.Average(r => (float)r.Rating) : 0,
                    ChannelName = s.Channel.Name,
                    CategoryName = s.Category.Name,
                    ViewsCount = s.ViewsCount,
                    LikesCount = s.LikesCount,
                    DislikesCount = s.DislikesCount,
                    ChannelImage = string.IsNullOrEmpty(s.Channel.Image) ?
                    $"http://readersclub.runasp.net//Uploads/ChannelsImages/welcome-image.jpeg" :
                    $"http://readersclub.runasp.net//Uploads/ChannelsImages/{s.Channel.Image}"

                })
                 .Distinct()
                .ToList();

            return allstories;
        }
        public StoryDto GetStoryById(int id)
        {
            var story = _context.Stories
                .Include(s => s.Channel)
                .Include(s => s.Category)
                .Where(s => s.IsValid && !s.IsDeleted && s.IsActive && s.Status == Status.Approved)
                .Select(s => new StoryDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    Cover = string.IsNullOrEmpty(s.Cover) ? null :
$"http://readersclub.runasp.net//Uploads/Covers/{s.Cover}",
                    Description = s.Description,
                    AverageRating = s.Reviews.Any() ? s.Reviews.Average(r => (float)r.Rating) : 0,
                    ChannelName = s.Channel.Name,
                    CategoryName = s.Category.Name,
                    File = $"http://readersclub.runasp.net//Uploads/pdfs/{s.File}",
                    Audio = $"http://readersclub.runasp.net//Uploads/audios/{s.Audio}",
                    Summary = s.Summary,
                    ViewsCount = s.ViewsCount,
                    LikesCount = s.LikesCount,
                    DislikesCount = s.DislikesCount
                })
                .FirstOrDefault(s => s.Id == id);
            return story;
        }
        public void UpdateStoryViewsCount(int storyId)
        {
            var story = _context.Stories.FirstOrDefault(s => s.Id == storyId);
            if (story != null)
            {
                story.ViewsCount++;
                _context.SaveChanges();
            }
        }
        public void UpdateStoryLikesCount(int storyId)
        {
            var story = _context.Stories.FirstOrDefault(s => s.Id == storyId);
            if (story != null)
            {
                story.LikesCount++;
                _context.SaveChanges();
            }
        }
        public void UpdateStoryUnlikesCount(int storyId)
        {
            var story = _context.Stories.FirstOrDefault(s => s.Id == storyId);
            if (story != null)
            {
                story.LikesCount--;
                _context.SaveChanges();
            }
        }
        public void UpdateStoryDislikesCount(int storyId)
        {
            var story = _context.Stories.FirstOrDefault(s => s.Id == storyId);
            if (story != null)
            {
                story.DislikesCount++;
                _context.SaveChanges();
            }
        }
        public void UpdateStoryUnDislikesCount(int storyId)
        {
            var story = _context.Stories.FirstOrDefault(s => s.Id == storyId);
            if (story != null)
            {
                story.DislikesCount--;
                _context.SaveChanges();
            }
        }
        public bool ToggleSaveStory(int storyId, int userId)
        {
            var existing = _context.SavedStories
                .FirstOrDefault(s => s.StoryId == storyId && s.UserId == userId);

            if (existing != null)
            {
                _context.SavedStories.Remove(existing);
                _context.SaveChanges();
                return false; // story removed from saved
            }

            var saved = new SavedStories
            {
                StoryId = storyId,
                UserId = userId,
                IsSaved = true
            };

            _context.SavedStories.Add(saved);
            _context.SaveChanges();
            return true; // story saved
        }
        public bool IsStorySaved(int storyId, int userId)
        {
            return _context.SavedStories.Any(s => s.StoryId == storyId && s.UserId == userId);
        }

        public async Task<List<StoryDto>> GetSavedStoriesByUserId(int userId)

        {

            var savedStories = await _context.SavedStories

                .Where(ss => ss.UserId == userId)

                .Include(ss => ss.Story)

                    .ThenInclude(s => s.Category)

                .Select(ss => new StoryDto

                {

                    Id = ss.Story.Id,

                    Title = ss.Story.Title,

                    Description = ss.Story.Description,

                    Cover = string.IsNullOrEmpty(ss.Story.Cover) ? null :

            $"http://readersclub.runasp.net//Uploads/Covers/{ss.Story.Cover}",

                    CategoryName = ss.Story.Category.Name,

                    ViewsCount = ss.Story.ViewsCount,

                })
                .ToListAsync();
            return savedStories;

        }

        public async Task<List<StoryDto>> FilterStory(string? storyTitle, string? category, string? writerName)
        {
            if (string.IsNullOrEmpty(storyTitle) && string.IsNullOrEmpty(category) && string.IsNullOrEmpty(writerName))
            {
                return GetAllStories();
            }
            else
            {
                var stories = await _context.Stories
                    .Include(s => s.Channel)
                    .Include(s => s.Category)
                    .Where(s => s.IsValid && !s.IsDeleted && s.IsActive && s.Status == Status.Approved)
                    .ToListAsync();

                if (!string.IsNullOrEmpty(storyTitle))
                {
                    stories = stories.Where(s => s.Title.ToLower().Contains(storyTitle.ToLower())).ToList();
                }
                if (!string.IsNullOrEmpty(category))
                {
                    stories = stories.Where(s => s.Category.Name.ToLower().Contains(category.ToLower())).ToList();
                }
                if (!string.IsNullOrEmpty(writerName))
                {
                    stories = stories.Where(s => s.User.Name.ToLower().Contains(writerName.ToLower())).ToList();
                }

                return stories.Select(s => new StoryDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    Cover = $"http://readersclub.runasp.net//Uploads/Covers/{s.Cover}",
                    Description = s.Description,
                    Summary = s.Summary,
                    AverageRating = s.Reviews.Any() ? s.Reviews.Average(r => (float)r.Rating) : 0,
                    ChannelName = s.Channel.Name,
                    CategoryName = s.Category.Name,
                    ViewsCount = s.ViewsCount,
                    LikesCount = s.LikesCount,
                    DislikesCount = s.DislikesCount,

                }).ToList();
            }
        }
    }
}