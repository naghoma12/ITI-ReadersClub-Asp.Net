using ReadersClubCore.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ReadersClubApi.DTO
{
    public class ReviewDto
    {
        public int Id { get; set; }
        [MaxLength(500)]
        public string Comment { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }

        public int StoryId { get; set; }
        [Required]
        public int UserId { get; set; }

        public string? UserName { get; set; }
        public string? UserImage { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}
