using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReadersClubApi.DTO;
using ReadersClubApi.Helpers;
using ReadersClubCore.Data;
using ReadersClubCore.Models;

namespace ReadersClubApi.Services
{
    public class ProfileService
    {
        private readonly ReadersClubContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration configuration;

        public ProfileService(ReadersClubContext context, UserManager<ApplicationUser> userManager
            ,IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            this.configuration = configuration;
        }
        //change password
        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return false;

            if (dto.NewPassword != dto.ConfirmPassword)
                return false;

            var result = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);

            return result.Succeeded;
        }

        //get profile
        public async Task<ProfileDto?> GetUserProfileAsync(int userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "reader";

            var profile = new ProfileDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Image = string.IsNullOrEmpty(user.Image) ?
                    $"http://readersclub.runasp.net//Uploads//Users/not_found.jpg":
                $"{configuration["ApiURL"]}//Uploads/Users/{user.Image}" ,
                Phone = user.PhoneNumber,
                Role = role
            };

            if (role == "reader")
            {
                profile.SavedStories = await _context.SavedStories
                    .Where(s => s.UserId == user.Id)
                    .Select(s => new SavedStoryDto
                    {
                        Id = s.Story.Id,
                        Title = s.Story.Title,
                        Image = $"http://readersclub.runasp.net//Uploads/Covers/{s.Story.Cover}"
                        
                    }).ToListAsync();

                profile.LastViewedStories = await _context.ReadingProgresses
                    .Where(rp => rp.UserId == user.Id)
                    .OrderByDescending(rp => rp.LastPage)
                    .Take(6)
                    .Select(rp => new LastViewedStoryDto
                    {
                        Id = rp.Story.Id,
                        Title = rp.Story.Title
                    }).ToListAsync();
            }
            else if (role == "author")
            {
                profile.Channels = await _context.Channels
                    .Where(c => c.UserId == user.Id)
                    .Select(c => new ChannelDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Image = string.IsNullOrEmpty(c.Image) ?
                    $"http://readersclub.runasp.net//Uploads/ChannelsImages/welcome-image.jpeg" :
                    $"http://readersclub.runasp.net//Uploads/ChannelsImages/{c.Image}"
                    }).ToListAsync();

                profile.PublishedStories = await _context.Stories
                    .Where(s => s.Channel.UserId == user.Id)
                    .Select(s => new PublishedStoryDto
                    {
                        Id = s.Id,
                        Title = s.Title
                    }).ToListAsync();

                profile.Reviews = await _context.Reviews
                    .Where(r => r.Story.Channel.UserId == user.Id)
                    .Select(r => new ReviewDto
                    {
                        Id = r.Id,
                        Rating = r.Rating,
                        Comment = r.Comment
                    }).ToListAsync();
            }

            return profile;
        }
        //update
        public async Task<bool> UpdateProfileAsync(int userId, UpdateProfileDto updateDto)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return false;

            user.Name = updateDto.Name;
            user.Image = updateDto.Image;
            user.PhoneNumber = updateDto.Phone;

            // غير الإيميل باستخدام UserManager
            var emailResult = await _userManager.SetEmailAsync(user, updateDto.Email);
            var usernameResult = await _userManager.SetUserNameAsync(user, updateDto.Email);

            if (!emailResult.Succeeded || !usernameResult.Succeeded)
                return false;

            var updateResult = await _userManager.UpdateAsync(user);

            return updateResult.Succeeded;
        }


        //update image

        public async Task<string?> UploadProfileImageAsync(int userId, IFormFile imageFile, string webEnv)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || imageFile == null || imageFile.Length == 0)
                return null;

            string uploadDir = Path.Combine(webEnv, "Uploads", "Users");

            if (!Directory.Exists(uploadDir))
                Directory.CreateDirectory(uploadDir);

            var photoName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
            var path = Path.Combine(uploadDir, photoName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            user.Image = photoName;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return photoName; // أو ترجعي لينك كامل لو حابة
        }

        //daelete account
        public async Task<bool> DeleteAccountAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
