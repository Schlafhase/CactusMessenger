namespace Messenger
{
    public interface IChannelRepository
    {
        Task<Channel> GetChannel(Guid channelId, Guid userId, IAccountRepository accountRepo);
        Task<Guid> CreateChannel(HashSet<Guid> userIds, Guid userId, IAccountRepository accountRepo);
        Task AddUser(Guid Id, Guid channelId, Guid userId, IAccountRepository accountRepo, IChannelRepository channelRepo);


	}
}
