namespace ReadersClubApi.DTO
{
    public class ChannelWithStoriesDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public ChannelOwnerDto Owner { get; set; }
        public List<StoryMiniDto> Stories { get; set; }


    }


}

