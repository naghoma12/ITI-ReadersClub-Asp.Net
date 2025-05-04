using System.ComponentModel.DataAnnotations;

namespace ReadersClubApi.DTO
{
    public class ForgetPasswordForm
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
