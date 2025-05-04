using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReadersClubCore.Data;
using ReadersClubCore.Models;
using ReadersClubDashboard.ViewModels;
using System.Data;
using System.Security.Claims;

namespace ReadersClubDashboard.Controllers
{
    public class AccountController : Controller
    {
        private readonly ReadersClubContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(ReadersClubContext context
            ,UserManager<ApplicationUser> userManager
            ,SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginedUser user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByNameAsync(user.UserName);
                bool IsValid = false;
                if (existingUser == null)
                {
                    ModelState.AddModelError("UserName","لا يوجد اسم مستخدم بهذا الاسم");
                    return View(user);
                }
                var userRoles = await _userManager.GetRolesAsync(existingUser);
                foreach (var role in userRoles)
                {
                    if(role == "author" || role == "admin")
                    {
                        IsValid = true;
                    }
                }
                if (!IsValid)
                {
                    ModelState.AddModelError("UserName", "لا يُسمح لك الدخول على لوحة التحكم .");
                    return View(user);
                }
                var result = await _signInManager.PasswordSignInAsync(user.UserName, user.Password, user.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var claims = new List<Claim>() { 
                        new Claim(ClaimTypes.NameIdentifier,existingUser.Id.ToString()),
                        new Claim(ClaimTypes.Name , existingUser.UserName),
                        new Claim(ClaimTypes.Role, userRoles.FirstOrDefault(r => r == "admin" || r == "author")),
                    };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                       principal, new AuthenticationProperties() { IsPersistent = user.RememberMe });
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "تسجيل دخول غير صحيح .");
                    return View(user);
                }
            }
            return View(user);
        }
        [Authorize]
        public async Task<IActionResult> LogOut()
        {
           await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
       
    }
}
