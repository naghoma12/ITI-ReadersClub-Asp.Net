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
    [HttpPost("Subscribe/{channelId}")]
    public async Task<IActionResult> Subscribe(int channelId)
    {
        try
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var result = await _channelService.Subscribe(channelId, userId);
            if (result)
                return Ok();

            return BadRequest("You are already subscribed to this channel");
        }
        catch (FormatException)
        {
            return Unauthorized("Invalid user identifier format.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while subscribing to the channel.");
        }

    }
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost("UnSubscribe/{channelId}")]
    public async Task<IActionResult> UnSubscribe(int channelId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();
        var result = await _channelService.UnSubscribe(channelId, int.Parse(userId));
        if (result)
            return Ok();
        return BadRequest("You are already subscribed to this channel");

    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("IsSubscribed/{channelId}")]
    public async Task<IActionResult> IsSubscribe(int channelId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();
        var result = await _channelService.IsSubscribe(channelId, int.Parse(userId));
       
            return Ok(result);
    }

}
