using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReadersClubApi.DTO;
using ReadersClubApi.Services;
using System.Security.Claims;

namespace ReadersClubApi.Controllers
{
    [Authorize]
    public class ProfileController : BaseController
    {
        private readonly ProfileService _profileService;
        private readonly IWebHostEnvironment _env;

        public ProfileController(ProfileService profileService
            ,IWebHostEnvironment env)
        {
            _profileService = profileService;
            _env = env;
        }

        [HttpGet("GetProfile")]
        public async Task<IActionResult> GetProfile()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
                return Unauthorized("Invalid user ID");

            var profile = await _profileService.GetUserProfileAsync(userId);
            if (profile == null)
                return NotFound("User not found");

            return Ok(profile);
        }


        //update profile
        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
                return Unauthorized("Invalid user ID");

            var result = await _profileService.UpdateProfileAsync(userId, dto);
            if (!result)
                return NotFound("User not found");

            return Ok(new { message = "Profile updated successfully" });
        }
        //change password

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
                return Unauthorized("Invalid user ID");

            var result = await _profileService.ChangePasswordAsync(userId, dto);
            if (!result)
                return BadRequest("Password change failed. Check your inputs.");

            return Ok(new { message = "Password changed successfully" });
        }
        //upload image

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage([FromForm] UploadImageDto dto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
                return Unauthorized("Invalid user ID");

            var imagePath = await _profileService.UploadProfileImageAsync(userId, dto.Image,_env.WebRootPath);
            if (imagePath == null)
                return BadRequest("Image upload failed");

            return Ok(new { imageUrl = imagePath });
        }
        //delete account
        [HttpDelete]
        public async Task<IActionResult> DeleteAccount()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
                return Unauthorized("Invalid user ID");

            var result = await _profileService.DeleteAccountAsync(userId);
            if (!result)
                return NotFound("User not found");

            return Ok("Account deleted successfully");
        }

    }
}
