using Microsoft.AspNetCore.Mvc;
using ReadersClubApi.Service;
using ReadersClubApi.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost("subscribe/{channelId}")]
    public async Task<IActionResult> Subscribe(int channelId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();
        var result = await _channelService.Subscribe(int.Parse(userId), channelId);
        if (result)
            return Ok();
        return BadRequest("You are already subscribed to this channel");

    }

}
