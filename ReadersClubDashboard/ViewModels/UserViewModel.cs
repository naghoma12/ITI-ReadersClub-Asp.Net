using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ReadersClubDashboard.ViewModels
{
    public class UserViewModel
    {
        [DisplayName("المعرِف")]
        public int Id { get; set; }
        [DisplayName("الإسم")]
        public string Name { get; set; }
        [DisplayName("اسم المستخدم")]
        public string UserName { get; set; }
        public List<string> Roles { get; set; }
        [DisplayName("رقم التليفون")]
        public string PhoneNumber { get; set; }
        [DisplayName("البريد الإلكتروني")]
        public string Email { get; set; }
        [DisplayName("الصوره")]
        public string? Image { get; set; }
    }
}
