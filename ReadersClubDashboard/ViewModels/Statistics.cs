namespace ReadersClubDashboard.ViewModels
{
    public class Statistics
    {
        public int StoriesCount { get; set; } = 0;
        public int ChannelsCount { get; set; } = 0;
        public int ReviewsCount { get; set; } = 0;
        public int AuthorsCount { get; set; } = 0;

        public bool NotificationFlag { get; set; } = false;
    }
}
