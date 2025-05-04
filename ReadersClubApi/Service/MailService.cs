
using Microsoft.AspNetCore.Identity;
using ReadersClubCore.Models;
using System.Net;
using System.Net.Mail;

namespace ReadersClubApi.Service
{
    public class MailService : IMailService
        {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        public MailService(IConfiguration configuration,UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var parts = email.Split('@');
            var domain = parts.Length > 1 ? parts[1] : string.Empty;
            var user = await _userManager.FindByEmailAsync(email);
            var smtpClient = new SmtpClient(_configuration["MailSettings:Host"])
            {
                Port = int.Parse(_configuration["MailSettings:Port"]),
                Credentials = new NetworkCredential(
        _configuration["MailSettings:UserName"],
        _configuration["MailSettings:Password"]
    ),
                EnableSsl = _configuration.GetValue<bool>("MailSettings:UseSSL"),
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["MailSettings:From"]),
                Subject = subject,
                Body = $"<div class=\"jumbotron\">\r\n  <h1 class=\"display-4\">Hello,{user.UserName}!</h1>\r\n <p>We received a request to reset your password for your ReadersClub account. To proceed with changing your password, please use the verification code below:</p>\r\n <h1 class=\"lead\">Your Verification Code: {message}</h1>" +
                $"\r\n</div><p>This code is valid for the next 10 minutes. If you did not request a password reset, please ignore this email or contact our support team immediately.</p> <hr/> \r\n" +
                $"<p>Thank you for using ReadersClub!\r\n\r\n</p> \r\n <p>Best regards,</p>\r\n**ReadersClub Support Team**",
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);

        }
    }
    }