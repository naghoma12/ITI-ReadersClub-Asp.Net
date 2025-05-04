using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReadersClubApi.DTO;
using ReadersClubApi.Helper;
using ReadersClubApi.Service;
using ReadersClubCore.Models;
using System.ComponentModel.DataAnnotations;

namespace ReadersClubApi.Controllers
{

    public class SecurityController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TokenConfiguration _token;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMailService _mailService;

        public SecurityController(UserManager<ApplicationUser> userManager
            ,TokenConfiguration token
            ,SignInManager<ApplicationUser> signInManager
            ,IMailService mailService)
            
        {
            _userManager = userManager;
            _token = token;
            _signInManager = signInManager;
           _mailService = mailService;
        }
        //Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegiserForm regiserForm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await _userManager.FindByEmailAsync(regiserForm.Email);
                    if (existingUser != null)
                    {
                        return BadRequest("البريد الإلكتروني مستخدم بالفعل");
                    }
                    var user = new ApplicationUser()
                    {
                        Name = regiserForm.Name,
                        Email = regiserForm.Email,
                        UserName = regiserForm.Email.Split('@').FirstOrDefault()
                    };
                    var result = await _userManager.CreateAsync(user, regiserForm.Password);
                    if (result.Succeeded)
                    {
                        if (regiserForm.IsAuthor)
                        {
                            await _userManager.AddToRoleAsync(user, "author");
                        }
                        else
                        {
                            await _userManager.AddToRoleAsync(user, "reader");
                        }
                        return Ok(new
                        {
                            Message = "تم التسجيل بنجاح",
                            Token = _token.CreateToken(user, _userManager).Result
                        });
                    }
                    ModelState.AddModelError("UserName", "اسم المستخدم موجود بالفعل .");
                }
                catch (Exception ex)
                {
                    return BadRequest(new
                    {
                        Message = "فشل التسجيل",
                        UserData = regiserForm,
                        Error = ex.Message?? ex.InnerException?.Message
                    });
                }
            }
            return BadRequest(new
            {
                Message = "فشل التسجيل",
                UserData = regiserForm,
                Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
            });

        }

        //Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginForm loginForm)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginForm.Email);
                if(user == null)
                {
                    return BadRequest("البريد الإلكتروني غير صحيح");
                }
                var result = await _signInManager.PasswordSignInAsync(user, loginForm.Password, loginForm.RememberMe, lockoutOnFailure: false);
                if(result.Succeeded)
                {
                    return Ok(new
                    {
                        Message = "تم تسجيل الدخول بنجاح",
                        Token = _token.CreateToken(user, _userManager).Result
                    });
                }
                return BadRequest("كلمة المرور غير صحيحة"); 
            }
            return BadRequest(new
            {
                Message = "فشل تسجيل الدخول",
                UserData = loginForm,
                Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
            });
        }

        //LogOut
        [HttpPost("LogOut")]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return Ok(new
            {
                Message = "تم تسجيل الخروج بنجاح"
            });
        }
        //Forget Password
        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordForm forgetPasswordForm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByEmailAsync(forgetPasswordForm.Email);
                    if (user == null)
                    {
                        return BadRequest("لا يوجد مستخدم بهذا البريد الإلكتروني");
                    }

                    var OTP = new Random().Next(100000, 999999).ToString();

                    await _mailService.SendEmailAsync(user.Email, "ReadersClub - Reset Password", OTP);
                    return Ok(OTP);
                }
                catch (Exception ex)
                {
                    return BadRequest("فشلت عملية إرسال الكود");
                }
            }
            return BadRequest("فشلت عملية إرسال الكود");

        }
       
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordForm resetPasswordForm)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(resetPasswordForm.Email);
                if (user == null)
                {
                    return BadRequest("لا يوجد مستخدم بهذا البريد الإلكتروني");
                }
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest(new { Message = "فشل في إنشاء رمز إعادة تعيين كلمة المرور" });
                }
                var result = await _userManager.ResetPasswordAsync(user, token, resetPasswordForm.NewPassword);
                if (result.Succeeded)
                {
                    return Ok(new
                    {
                        Message = "تم تغيير كلمة المرور بنجاح",
                        Token = token
                    });
                }
                
            }
            return BadRequest(new
            {
                Message = "فشلت عملية تغيير كلمة المرور"
            });
        }
    }
}