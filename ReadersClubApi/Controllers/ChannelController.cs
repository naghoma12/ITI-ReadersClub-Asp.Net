using Microsoft.AspNetCore.Mvc;
using ReadersClubApi.Service;
using ReadersClubApi.DTO;

[ApiController]
[Route("api/[controller]")]
public class ChannelController : ControllerBase
{
    private readonly IChannelService _channelService;

    public ChannelController(IChannelService channelService)
    {
        _channelService = channelService;
    }

    [HttpGet]
    public ActionResult<List<ChannelWithStoriesDto>> GetAll()
    {
        var channels = _channelService.GetAllChannels();
        return Ok(channels);
    }
    [HttpGet("{id}")]
    public ActionResult<ChannelWithStoriesDto> GetById(int id)
    {
        var channel = _channelService.GetChannelById(id);
        if (channel == null)
            return NotFound();
        return Ok(channel);
    }

}
