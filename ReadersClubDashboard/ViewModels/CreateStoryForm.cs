using ReadersClubCore.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ReadersClubDashboard.ViewModels
{
    public class CreateStoryForm
    {
        [MaxLength(200)]
        [Required(ErrorMessage ="يجب ادخال عنوان الروايه")]
        [Display(Name = "عنوان الروايه")]
        public string Title { get; set; }
        public IFormFile coverFile { get; set; } //For Cover Image
        public string? Cover { get; set; }
        [Required(ErrorMessage = "يجب ادخال وصف الروايه")]
        [Display(Name = "وصف الروايه")]
        public string Description { get; set; }
        [RegularExpression(@"^.*\.(pdf|PDF)$", ErrorMessage = "Only PDF files are allowed.")]
        public string? File { get; set; }
        public IFormFile pdfFile { get; set; } //For PDF

        [RegularExpression(@"^.*\.(mp3|MP3)$", ErrorMessage = "Only MP3 audios are allowed.")]
        public string? Audio { get; set; }
        public IFormFile audioFile { get; set; } //For Audio
        [Required(ErrorMessage = "يجب ادخال ملخص الروايه")]
        [Display(Name = "ملخص الروايه")]
        public string Summary { get; set; }
        public bool IsActive { get; set; } = true;
        public Status Status { get; set; } = Status.Pending;
        public bool IsValid { get; set; } = false;  //Should be true , Admin determine accept story or not
        [Required]
        public int CategoryId { get; set; }
        public int UserId { get; set; }
        [Required]
        public int ChannelId { get; set; }
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public IEnumerable<Channel> Channels { get; set; } = new List<Channel>();
        public IEnumerable<ApplicationUser> ApplicationUsers { get; set; } = new List<ApplicationUser>();
    }
}

