namespace ReadersClubApi.DTO
{
    public class ProfileDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }

        // Reader specific
        public List<SavedStoryDto>? SavedStories { get; set; }
        public List<LastViewedStoryDto>? LastViewedStories { get; set; }

        // Author specific
        public List<ChannelDto>? Channels { get; set; }
        public List<PublishedStoryDto>? PublishedStories { get; set; }
        public List<ReviewDto>? Reviews { get; set; }
    }
}
