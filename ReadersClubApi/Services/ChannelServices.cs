using Microsoft.EntityFrameworkCore;
using ReadersClubApi.DTO;
using ReadersClubApi.Service;
using ReadersClubCore.Data;
using ReadersClubCore.Models;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;


namespace ReadersClubApi.Services
{
    public class ChannelService : IChannelService
    {
        private readonly ReadersClubContext _context;

        public ChannelService(ReadersClubContext context)
        {
            _context = context;
        }

        public List<ChannelWithStoriesDto> GetAllChannels()
        {
            var channels = _context.Channels
                .Include(c => c.Stories)
                .Include(c => c.User)
                .Where(x => x.IsDeleted == false)
                .Select(c => new ChannelWithStoriesDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Image = string.IsNullOrEmpty(c.Image) ?
                    $"http://readersclub.runasp.net//Uploads/ChannelsImages/welcome-image.jpeg" :
                    $"http://readersclub.runasp.net//Uploads/ChannelsImages/{c.Image}",
                    Owner = new ChannelOwnerDto
                    {
                        Id = c.User.Id,
                        UserName = c.User.Name
                    },
                    Stories = c.Stories.Select(s => new StoryMiniDto
                    {
                        Title = s.Title,
                        Image = $"http://readersclub.runasp.net//Uploads/{s.Cover}",
                        Category = s.Category.Name
                    }).ToList()
                }).ToList();

            return channels;
        }

        public ChannelWithStoriesDto GetChannelById(int id)
        {
            var channels = _context.Channels
                .Include(c => c.Stories)
                .Include(c => c.User)
                .Where(x => x.IsDeleted == false)
                .Select(c => new ChannelWithStoriesDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Image = string.IsNullOrEmpty(c.Image) ?
                    $"http://readersclub.runasp.net//Uploads/ChannelsImages/welcome-image.jpeg" :
                    $"http://readersclub.runasp.net//Uploads/ChannelsImages/{c.Image}",
                    Owner = new ChannelOwnerDto
                    {
                        Id = c.User.Id,
                        UserName = c.User.Name
                    },
                    Stories = c.Stories.Select(s => new StoryMiniDto
                    {
                        Id = s.Id,
                        Title = s.Title,
                        Image = $"http://readersclub.runasp.net//Uploads/Covers/{s.Cover}",
                        Category = s.Category.Name
                    }).ToList()
                }).FirstOrDefault(x => x.Id == id);

            return channels;
        }
        public async Task<bool> Subscribe(int channelId, int userId)
        {
            var existingSubscription = await _context.Subscribtions
                        .FirstOrDefaultAsync(s => s.UserId == userId && s.ChannelId == channelId);

            if (existingSubscription != null)
                return false;

            var subscription = new Subscribtion
            {
                ChannelId = channelId,
                UserId = userId
            };
            _context.Subscribtions.Add(subscription);
            var flag = await _context.SaveChangesAsync();
            if (flag > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> UnSubscribe(int channelId, int userId)
        {
            var subscribtion = await _context.Subscribtions
           .FirstOrDefaultAsync(s => s.ChannelId == channelId && s.UserId == userId);

            if (subscribtion == null)
            {
                return false; // Nothing to remove
            }

            _context.Subscribtions.Remove(subscribtion);
            var flag = await _context.SaveChangesAsync();
            return flag > 0;
        }
    
        public async Task<bool> IsSubscribe(int channelId, int userId)
        {
            var subscribtion = await _context.Subscribtions
                .FirstOrDefaultAsync(x => x.ChannelId == channelId && x.UserId == userId);
            if (subscribtion != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    

}