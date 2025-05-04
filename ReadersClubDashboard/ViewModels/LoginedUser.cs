using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ReadersClubDashboard.ViewModels
{
    public class LoginedUser
    {
        [Required(ErrorMessage = "يجب ادخال اسم المستخدم")]
        [Display(Name = "اسم المستخدم")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "يجب ادخال الرقم السري")]
        [DataType(DataType.Password)]
        [DisplayName("الرقم السري")]
        public string Password { get; set; }
        [Display(Name = "تذكرني")]
        public bool RememberMe { get; set; }
    }
}
