using Microsoft.EntityFrameworkCore;
using ReadersClubApi.DTO;
using ReadersClubApi.Service;
using ReadersClubCore.Data;
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
                        Image =$"http://readersclub.runasp.net//Uploads/{s.Cover}",
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
                        Title = s.Title,
                        Image = $"http://readersclub.runasp.net//Uploads/Covers/{s.Cover}",
                        Category = s.Category.Name
                    }).ToList()
                }).FirstOrDefault(x => x.Id == id);

            return channels;
        }

    }

}