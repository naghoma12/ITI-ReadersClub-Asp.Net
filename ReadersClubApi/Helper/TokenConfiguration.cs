using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ReadersClubCore.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ReadersClubApi.Helper
{
    public class TokenConfiguration
    {
        private readonly IConfiguration _configuration;

        public TokenConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string> CreateToken(ApplicationUser user, UserManager<ApplicationUser> userManager)
        {
            #region Pivate Claims

            var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // User ID claim
                new Claim(ClaimTypes.Email, user.Email), // Email claim

            };

            var userRoles = await userManager.GetRolesAsync(user);

            foreach (var role in userRoles)
                authClaims.Add(new Claim(ClaimTypes.Role, role));

            #endregion

            #region Secret Key

            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));

            #endregion

            #region Token Object & Registered Claims

            var token = new JwtSecurityToken(

              issuer: _configuration["JWT:Issuer"],
              audience: _configuration["JWT:Audience"],
              expires: DateTime.UtcNow.AddDays(double.Parse(_configuration["JWT:DurationExpire"])),
              claims: authClaims,
              signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256Signature)
              );

            #endregion

            //Token itself
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
