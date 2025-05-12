using System.ComponentModel.DataAnnotations;

namespace ReadersClubApi.DTO
{
    public class AddReviewDto
    {
        [MaxLength(500)]
        public string Comment { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public int StoryId { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
