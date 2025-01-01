using System.ComponentModel.DataAnnotations;
using MessengerInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Controllers;

//[ApiController]
//[Route("[controller]")]
public class MessengerController : ControllerBase
{
	private readonly ILogger<MessengerController> _logger;
	private readonly IMessengerService messengerService;


	public MessengerController(ILogger<MessengerController> logger, IMessengerService messengerService)
	{
		_logger = logger;
		this.messengerService = messengerService;
	}

	[HttpPost("editAccountAdminStatus")]
	public async Task<ActionResult> EditAccountAdminStatus([FromQuery] [Required] bool giveAdmin,
		[FromBody] [Required] Guid Id = default)
	{
		try
		{
			await messengerService.EditAccountAdmin(Id, giveAdmin);
			return Ok();
		}
		catch (Exception ex)
		{
			return BadRequest(ex.Message);
		}
	}

	[HttpGet("getAllMessages")]
	public async Task<ActionResult<MessageDTO_Output[]>> GetAllMessages()
	{
		try
		{
			return await messengerService.GetAllMessages();
		}
		catch (Exception ex)
		{
			return BadRequest(ex.Message);
		}
	}

	[HttpGet("{channelId}/getAllMessages")]
	public async Task<ActionResult<MessageDTO_Output[]>> GetAllMessagesInChannel(
		[FromRoute] [Required] Guid channelId)
	{
		try
		{
			return await messengerService.GetAllMessagesInChannel(channelId);
		}
		catch (Exception ex)
		{
			return NotFound(ex.Message);
		}
	}

	[HttpGet("getAccount")]
	public async Task<ActionResult<Account>> GetAccount([FromQuery] [Required] Guid Id)
	{
		try
		{
			return await messengerService.GetAccount(Id);
		}
		catch (Exception ex)
		{
			return NotFound(ex.Message);
		}
	}

	[HttpGet("getChannel")]
	public async Task<ActionResult<ChannelDTO_Output>> GetChannel([FromQuery] [Required] Guid channelId)
	{
		try
		{
			ChannelDTO_Output channel = await messengerService.GetChannel(channelId);
			return channel;
		}
		catch (Exception ex)
		{
			return NotFound(ex.Message);
		}
	}

	[HttpPost("{channelId}/addUser")]
	public async Task<ActionResult> AddUser([FromBody] [Required] Guid Id,
		[FromRoute] [Required] Guid channelId)
	{
		try
		{
			await messengerService.AddUserToChannel(Id, channelId);
			return Ok();
		}
		catch (Exception ex)
		{
			return BadRequest(ex.Message);
		}
	}

	[HttpPost("{channelId}/postMessage")]
	public async Task<ActionResult> PostMessage([FromBody] [Required] MessageDTO_Input msg,
		[FromRoute] [Required] Guid channelId,
		[FromQuery] [Required] Guid userId)
	{
		try
		{
			Message message = msg.ToMessage(userId, channelId);
			await messengerService.PostMessage(message);
			return Ok();
		}
		catch (Exception ex)
		{
			return BadRequest(ex.Message);
		}
	}

	[HttpPost("createAccount")]
	public async Task<ActionResult<Guid>> CreateAccount([FromBody] [Required] string username, string passwordHash)
	{
		try
		{
			return await messengerService.CreateAccount(username, passwordHash);
		}
		catch (Exception ex)
		{
			return BadRequest(ex.Message);
		}
	}

	[HttpPost("createChannel")]
	public async Task<ActionResult<Guid>> CreateChannel([FromBody] [Required] HashSet<Guid> userIds)
	{
		try
		{
			return await messengerService.CreateChannel(userIds, "");
		}
		catch (Exception ex)
		{
			return BadRequest(ex.Message);
		}
	}
}