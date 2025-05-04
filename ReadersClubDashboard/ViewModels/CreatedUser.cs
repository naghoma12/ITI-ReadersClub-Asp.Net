using Microsoft.AspNetCore.Identity;
using ReadersClubCore.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ReadersClubDashboard.ViewModels
{
    public class CreatedUser
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="يجب ادخال الإسم")]
        [MaxLength(50,ErrorMessage = "عدد الحروف يجب ألا يتخطى عن 50 حرف")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "الإسم يجب ان يحتوي فقط على حروف ومسافات")]
        [DisplayName("الإسم")]
        public string Name { get; set; }
        [Required(ErrorMessage = "يجب ادخال اسم المستخدم")]
        [DisplayName("اسم المستخدم")]
        public string UserName { get; set; }
        [DisplayName("البريد الإلكتروني")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "يجب ادخال رقم التليفون")]
        [DisplayName("رقم التليفون")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "يجب ادخال الرقم السري")]
        [DisplayName("الرقم السري")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [RegularExpression(@"^.*\.(jpg|jpeg|png)$",
     ErrorMessage = ".(jpg, jpeg, png) ملفات الصور المتاحه ")]
        [DisplayName("الصوره")]
        public string? Image { get; set; }

        public IFormFile? formFile { get; set; } 
        public string? ImagePath { get; set; }
        public int? RoleId { get; set; } = 3;
        public ICollection<ApplicationRole> Roles { get; set; }= new List<ApplicationRole>();
    }
}
