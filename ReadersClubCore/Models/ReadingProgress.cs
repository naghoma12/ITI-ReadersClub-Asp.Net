using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadersClubCore.Models
{
    //Many to Many Relation between User and Story
    public class ReadingProgress:BaseEntity
    {
        public int StoryId { get; set; }
        public Story Story { get; set; }
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int LastPage { get; set; } = 0;
    }
}
