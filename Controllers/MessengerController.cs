using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Messenger.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessengerController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<MessengerController> _logger;
        private readonly IAccountRepository accountRepo;
        private readonly IChannelRepository channelRepo;
        private readonly IMessageRepository messageRepo;


        public MessengerController(ILogger<MessengerController> logger, IAccountRepository accountRepo, IChannelRepository channelRepo, IMessageRepository messageRepo)
        {
            _logger = logger;
            this.accountRepo = accountRepo;
            this.channelRepo = channelRepo;
            this.messageRepo = messageRepo;
        }

        [HttpPost("editAccountAdminStatus")]
        public async Task<ActionResult> EditAccountAdminStatus([FromQuery, Required] bool giveAdmin, [FromQuery, Required] Guid userId = default, [FromBody, Required] Guid Id = default)
        {
            try
            {
                accountRepo.EditAccountAdmin(Id, giveAdmin, userId, accountRepo);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getAllMessages")]
        public async Task<ActionResult<Message[]>> GetAllMessages([FromQuery, Required] Guid userId)
        {
            try
            {
                return await messageRepo.GetAllMessages(userId, accountRepo, channelRepo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{channelId}/getAllMessages")]
        public async Task<ActionResult<Message[]>> GetAllMessagesInChannel([FromRoute, Required]Guid channelId, [FromQuery, Required] Guid userId)
        {
            try
            {
                return await messageRepo.GetAllMessagesInChannel(channelId, userId, accountRepo, channelRepo);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("getAccount")]
        public async Task<ActionResult<Account>> GetAccount([FromQuery, Required] Guid Id, [FromQuery, Required] Guid userId)
        {
            try
            {
                return await accountRepo.GetAccount(Id);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        
        [HttpGet("getChannel")]
        public async Task<ActionResult<Channel>> GetChannel([FromQuery, Required] Guid channelId, [FromQuery, Required] Guid userId)
        {
            try
            {
                Channel channel = await channelRepo.GetChannel(channelId, userId, accountRepo);
                return channel;
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("{channelId}/addUser")]
        public async Task<ActionResult> AddUser([FromBody, Required] Guid Id, [FromRoute, Required] Guid channelId, [FromQuery, Required] Guid userId)
        {
            try
            {
                await channelRepo.AddUser(Id, channelId, userId, accountRepo, channelRepo);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{channelId}/postMessage")]
        public async Task<ActionResult> PostMessage([FromBody, Required] MessageDTO_Input msg, [FromRoute, Required] Guid channelId, [FromQuery, Required] Guid userId)
        {
            try
            {
                Message message = new Message(msg.Content, DateTime.UtcNow, userId, channelId);
                await messageRepo.PostMessage(message, userId, accountRepo, channelRepo);
                return Ok();
            }
            catch(Exception ex) 
            { 
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("createAccount")]
        public async Task<ActionResult<Guid>> CreateAccount([FromBody, Required] string username)
        {
            try
            {
                return await accountRepo.CreateAccount(username);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("createChannel")]
        public async Task<ActionResult<Guid>> CreateChannel([FromBody, Required] HashSet<Guid> userIds, [FromQuery, Required] Guid userId)
        {
            try
            {
                return await channelRepo.CreateChannel(userIds, userId, accountRepo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}