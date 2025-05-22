namespace ReadersClubApi.DTO
{
    public class PopularStoryDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Cover { get; set; }
        public float AverageRating { get; set; }

        public string ChannelName { get; set; }
        public string ChannelImage { get; set; }
        public string CategoryName { get; set; }
        public long ViewsCount { get; set; }
    }
}
