namespace Messenger
{
    /// <summary>
    /// Repository for messages.
    /// </summary>
    public interface IMessageRepository
    {
        Task PostMessage(Message message, Guid userId, IAccountRepository accountRepo, IChannelRepository channelRepo);
        Task<Message> GetMessage(Guid Id, Guid userId, IAccountRepository accountRepo, IChannelRepository channelRepo);
        Task<Message[]> GetAllMessages(Guid userId, IAccountRepository accountRepo, IChannelRepository channelRepo);
        Task<Message[]> GetAllMessagesInChannel(Guid channelId, Guid userId, IAccountRepository accountRepo, IChannelRepository channelRepo);
        //Task PostMessage(Message message, Guid channelId);

    }
}
