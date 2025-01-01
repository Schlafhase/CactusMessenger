using Messenger;
using MessengerInterfaces;

namespace CactusFrontEnd.FrontEndFunctions;

public class MessengerFunctions
{
	public static async Task SendMessage(string content, Guid channelId, Guid userId, IMessageService messageService)
	{
		MessageDTO_Input msg = new(content);
		await messageService.PostMessage(msg.ToMessage(userId, channelId));
	}

	public static async Task<MessageDTO_Output[]> GetMessages(Guid channelId,
		Guid userId,
		IMessageService messageService) =>
		await messageService.GetAllMessagesInChannel(channelId);

	public static Guid TryParseGuid(string guid)
	{
		Guid id;

		try
		{
			id = Guid.Parse(guid);
		}
		catch
		{
			id = CactusConstants.DeletedId;
		}

		return id;
	}
}