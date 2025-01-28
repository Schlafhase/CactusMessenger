namespace MessengerInterfaces;

/// <summary>
///     Repository for messages.
/// </summary>
public interface IMessageService
{
	event Action<ChannelDTO_Output> OnMessage;
	Task PostMessage(Message message);
	Task<MessageDTO_Output> GetMessage(Guid Id);
	Task<MessageDTO_Output[]> GetAllMessages();

	Task<MessageDTO_Output[]> GetAllMessagesInChannel(Guid channelId);
	Task<List<MessageDTO_Output>> GetMessagesByAccount(Guid accountId);

	//Task PostMessage(Message message, Guid channelId);
	Task DeleteMessage(Guid id);
	Task DeleteAllMessages();
}