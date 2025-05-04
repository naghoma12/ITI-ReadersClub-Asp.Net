using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReadersClubCore.Models
{
    public class Story : BaseEntity
    {
        [MaxLength(200)]
        [Required]
        public string Title { get; set; }
        public string Cover { get; set; }
        public string Description { get; set; }
        [RegularExpression(@"^.*\.(pdf|PDF)$", ErrorMessage = "Only PDF files are allowed.")]
        public string File { get; set; }
        [RegularExpression(@"^.*\.(mp3|MP3)$", ErrorMessage = "Only MP3 audios are allowed.")]
        public string Audio { get; set; }
        public string Summary { get; set; }
        public bool IsActive { get; set; } = true;//Writer 
        public long ViewsCount { get; set; } = 0; // Default Value

        [Range(0, int.MaxValue)]
        public int LikesCount { get; set; } = 0;

        [Range(0, int.MaxValue)]
        public int DislikesCount { get; set; } = 0;

        public Status Status { get; set; } = Status.Pending;
        public bool IsValid { get; set; } = false;  //Should be true , Admin determine accept story or not
        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }
        public Channel Channel { get; set; }

        [ForeignKey("Channel")]
        [Required]
        public int ChannelId { get; set; }
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<ReadingProgress> ReadingProgresses { get; set; } = new List<ReadingProgress>();
        public ICollection<SavedStories> SavedStories { get; set; } = new List<SavedStories>();
    }
    public enum Status
    {
        Pending,
        Approved,
        Rejected
    }
}
