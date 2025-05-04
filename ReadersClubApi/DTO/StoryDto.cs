using ReadersClubCore.Models;

namespace ReadersClubApi.DTO
{
    public class StoryDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Cover { get; set; }
        public string Description { get; set; }
        public float AverageRating { get; set; }
        public string ChannelName { get; set; }
        public string CategoryName { get; set; }

        public string File { get; set; }
        public string Audio { get; set; }
        public string Summary { get; set; }
        public long ViewsCount { get; set; } = 0;
        public int LikesCount { get; set; } = 0;
        public int DislikesCount { get; set; } = 0;
        public IEnumerable<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();
    }

}