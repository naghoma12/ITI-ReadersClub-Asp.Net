using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReadersClubCore.Data;
using ReadersClubCore.Models;
using ReadersClubDashboard.Helper;
using ReadersClubDashboard.ViewModels;
using System.Data;

namespace ReadersClubDashboard.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IWebHostEnvironment _environment;

        public UserController(IMapper mapper
            , UserManager<ApplicationUser> userManager
            , RoleManager<ApplicationRole> roleManager,
            IWebHostEnvironment environment)
        {
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _environment = environment;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users
                 .Select(x => new UserViewModel()
                 {
                     Id = x.Id,
                     Name = x.Name,
                     UserName = x.UserName,
                     PhoneNumber = x.PhoneNumber,
                     Email = x.Email,
                     Image = x.Image,
                     Roles = new List<string>()
                 }).ToListAsync();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(user.Id.ToString()));
                user.Roles = roles.ToList();
            }
            return View(users);
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.Users
                .Where(x => x.Id == id)
                .Select(x => new UserViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    UserName = x.UserName,
                    PhoneNumber = x.PhoneNumber,
                    Email = x.Email,
                    Image = x.Image,
                    Roles = new List<string>()
                }).FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                return RedirectToAction("Index");
            }
            var roles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(user.Id.ToString()));
            user.Roles = roles.ToList();
            return View(user);
        }
        public async Task<IActionResult> Create()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            CreatedUser createdUser = new CreatedUser();
            createdUser.Roles = roles;
            return View(createdUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatedUser userViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (userViewModel.formFile != null)
                    {
                        userViewModel.Image = Guid.NewGuid().ToString() + Path.GetExtension(userViewModel.formFile.FileName);
                        userViewModel.Image = await FileSettings.UploadFile(userViewModel.formFile, "Users", _environment.WebRootPath);
                    }
                    var user = new ApplicationUser()
                    {
                        Name = userViewModel.Name,
                        UserName = userViewModel.UserName,
                        PhoneNumber = userViewModel.PhoneNumber,
                        Email = userViewModel.Email,
                        Image = userViewModel.Image
                    };

                    var result = await _userManager.CreateAsync(user, userViewModel.Password);
                    if (result.Succeeded)
                    {
                        var role = await _roleManager.FindByIdAsync(userViewModel.RoleId.ToString());
                        if (role != null)
                        {
                            var roleResult = await _userManager.AddToRoleAsync(user, role.Name);
                            if (!roleResult.Succeeded)
                            {
                                ModelState.AddModelError(string.Empty, "Failed to add role to the user.");
                                return RedirectToAction("Index");
                            }
                        }
                        return RedirectToAction("Index");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                catch
                {
                    ModelState.AddModelError(string.Empty, "Failed to add user");
                    var roles = await _roleManager.Roles.ToListAsync();
                    userViewModel.Roles = roles;
                    return View(userViewModel);
                }
            }
            var rolesList = await _roleManager.Roles.ToListAsync();
            userViewModel.Roles = rolesList;
            return View(userViewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var existedUser = await _userManager.Users
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
            if (existedUser == null)
            {
                return RedirectToAction("Index");
            }
            return View(existedUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationUser application, IFormFile? formFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(application.Id.ToString());
                    if (formFile != null)
                    {
                        if(user.Image != null)
                        {
                            FileSettings.DeleteFile("Users",user.Image,_environment.WebRootPath);

                        }
                        application.Image = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName);
                        application.Image = await FileSettings.UploadFile(formFile, "Users", _environment.WebRootPath);
                    }
                    user.Name = application.Name;
                    user.UserName = application.UserName;
                    user.PhoneNumber = application.PhoneNumber;
                    user.Email = application.Email;
                    user.Image = application.Image;
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                }
                catch
                {
                    ModelState.AddModelError(string.Empty, "Failed to update user");
                }
            }
            return View(application);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var existedUser = await _userManager.Users
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
            if (existedUser == null)
            {
                return RedirectToAction("Index");
            }
            return View(existedUser);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ApplicationUser application)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(application.Id.ToString());
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    var userPath = Path.Combine(_environment.WebRootPath, "Users", user.Image);
                    if (System.IO.File.Exists(userPath))
                    {
                        System.IO.File.Delete(userPath);
                    }
                return RedirectToAction("Index");
                }
                return View(application);

            }
            catch
            {
                return View(application);
            }
        }
    }
}