using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadersClubCore.Models
{
    public class ApplicationUser:IdentityUser<int>
    {
        [Required]
        [MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces.")]  // تحقق من أن الاسم يحتوي فقط على حروف ومسافات
        public string Name { get; set; }  //Regex for name
        [RegularExpression(@"^.*\.(jpg|jpeg|png)$",
       ErrorMessage = "Only image files (jpg, jpeg, png) are allowed.")]
        public string? Image { get; set; }
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Story> Stories { get; set; } = new List<Story>();
        public ICollection<Channel> Channels { get; set; } = new List<Channel>();
        public ICollection<Subscribtion> Subscribtions { get; set; } = new List<Subscribtion>();


    }
}
