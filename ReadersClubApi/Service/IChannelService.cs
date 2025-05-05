using ReadersClubApi.DTO;

namespace ReadersClubApi.Service
{
    public interface IChannelService
    {
        List<ChannelWithStoriesDto> GetAllChannels();
        ChannelWithStoriesDto GetChannelById(int id);
        Task<bool> Subscribe(int userId, int channelId);
    }
}
