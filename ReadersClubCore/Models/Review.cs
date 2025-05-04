using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadersClubCore.Models
{
    public class Review: BaseEntity
    {
        [MaxLength(500)]
        public string Comment { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }

        public int StoryId { get; set; }
        public Story? Story { get; set; }
        [ForeignKey("User")]
        [Required]
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
