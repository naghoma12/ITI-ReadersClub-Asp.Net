using System.ComponentModel.DataAnnotations;

namespace ReadersClubApi.DTO
{
    public class RegiserForm
    {
        [Required]
        [MaxLength(20)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces.")]
        public string Name { get; set; }
  
        public string? UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).{8,}$",
ErrorMessage = "كلمة المرور يجب أن تحتوي على حرف كبير، حرف صغير، رقم، وأن تكون على الأقل 8 أحرف.")]
        public string Password { get; set; }
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        public bool IsAuthor { get; set; } = false;
    }
}
