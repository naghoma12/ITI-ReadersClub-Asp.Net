using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ReadersClubCore.Models
{
    public class Channel : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        [RegularExpression(@"^.*\.(jpg|jpeg|png)$",
        ErrorMessage = "Only image files (jpg, jpeg, png) are allowed.")]
        public string? Image { get; set; }
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<Story> Stories { get; set; } = new List<Story>();
        public ICollection<Subscribtion> Subscribtions { get; set; } = new List<Subscribtion>();

    }
}
