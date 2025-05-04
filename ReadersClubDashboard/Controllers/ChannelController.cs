using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadersClubCore.Data;
using ReadersClubCore.Models;
using ReadersClubDashboard.Helper;
using ReadersClubDashboard.Sevice;
using static System.Net.Mime.MediaTypeNames;
namespace ReadersClubDashboard.Controllers
{
    //handling channels
    [Authorize]
    public class ChannelController : Controller
    {
        
        private readonly ReadersClubContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ChannelService _channelService;
        private readonly IWebHostEnvironment _environment;

        public ChannelController(ReadersClubContext context
            ,UserManager<ApplicationUser> userManager
            ,ChannelService channelService
            ,IWebHostEnvironment environment)
        {
            _context = context;
           _userManager = userManager;
            _channelService = channelService;
            _environment = environment;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index()
        {
            var channels = await _channelService.GetAllChannels();
                return View(channels);
           
        }
        [Authorize(Roles = "author")]
        public async Task<IActionResult> AuthorChannels()
        {
            var user = await _userManager.GetUserAsync(User);
            var channels = await _channelService.GetAuthorChannels(user.Id);
            return View("Index",channels);
            
        }
        public IActionResult Details(int id)
        {
            var channel = _channelService.GetChannel(id);
            return View(channel);
        }
        [HttpGet]
        public async Task<IActionResult> AddChannel()
        {
            ViewData["Users"] = await _userManager.Users.ToListAsync();
            return View("AddChannel");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddChannel(Channel channelData, IFormFile? imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                channelData.Image = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                channelData.Image = await FileSettings.UploadFile(imageFile, "ChannelsImages", _environment.WebRootPath);
            }

            // save data
            ModelState.Remove("User");
            if (ModelState.IsValid)
            {
                _context.Channels.Add(channelData);
                await _context.SaveChangesAsync();
                if (User.IsInRole("admin"))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("AuthorChannels");
                }
            }
            ViewData["Users"] = await _userManager.Users.ToListAsync();
            return View("AddChannel", channelData); // لو فيه error في الفاليديشن
        }

        [HttpGet]
        public IActionResult DeleteChannel(int id)
        {
            return Details(id);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteChannel(Channel channel)
        {
            try
            {
                _context.Channels.Remove(channel);
                _context.SaveChanges();
                if (User.IsInRole("admin"))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("AuthorChannels");
                }
            }
            catch
            {
                return View(channel);
            }
        }

        public async Task<IActionResult> EditChannel(int id)
        {
            var channel = _channelService.GetChannel(id);
            ViewData["Users"] = await _userManager.Users.ToListAsync();
            return View(channel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditChannel(int id, Channel channelData, IFormFile? imageFile)
        {
            var channel = _context.Channels.Find(id);
            if (imageFile != null && imageFile.Length > 0)
            {
                if(channel.Image != null)
                {
                    FileSettings.DeleteFile("ChannelsImages", channelData.Image, _environment.WebRootPath);

                }
                channel.Image = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                channel.Image = await FileSettings.UploadFile(imageFile, "ChannelsImages", _environment.WebRootPath);
            }

            channel.Name = channelData.Name;
            channel.Description = channelData.Description;
            channel.UserId = channelData.UserId;
            _context.Update(channel);
            _context.SaveChanges();
            if (User.IsInRole("admin"))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("AuthorChannels");
            }

        }
    }
}
