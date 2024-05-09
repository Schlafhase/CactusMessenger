
using Microsoft.AspNetCore.Components;

namespace Messenger
{
    public class InMemoryChannelRepository : IChannelRepository
    {
        public readonly Dictionary<Guid, Channel> channels = new();

        public InMemoryChannelRepository()
        {
            channels[Guid.Empty] = new Channel([], Guid.Empty);
        }
        public async Task<Guid> CreateChannel(HashSet<Guid> userIds, Guid userId, IAccountRepository accountRepo)
        {
            Account user = await accountRepo.GetAccount(userId);
            if (userIds.Contains(user.Id) || user.isAdmin)
            {
                Guid channelId = Guid.NewGuid();
                channels.Add(channelId, new Channel(userIds, channelId));
                return channelId;
            }
            else
            {
                throw new Exception("User must be member of the channel (Except the user has administrative permissions)");
            }
        }

        public async Task<Channel> GetChannel(Guid channelId, Guid userId, IAccountRepository accountRepo)
        {
            Account user = await accountRepo.GetAccount(userId);
            Channel channel;
            try
            {
                channel = channels[channelId];
            }
            catch (KeyNotFoundException)
            {
                throw new Exception($"Unable to find channel with Id {channelId}");
            }
            if (channel.Users.Contains(user.Id) || user.isAdmin)
            {
                    return channel;
            }
            else
            {
                throw new Exception("User has no permission to see the requested channel.");
            }
        }

		public async Task AddUser(Guid Id, Guid channelId, Guid userId, IAccountRepository accountRepo, IChannelRepository channelRepo)
		{
			Account user = await accountRepo.GetAccount(userId);
            Channel channel = await channelRepo.GetChannel(channelId, userId, accountRepo);
            if (channel.Users.Contains(user.Id) || user.isAdmin)
            {
                channel.Users.Add(Id);
            }
			else
			{
				throw new Exception("Can only add users to channels you are in (Except for Admin accounts)");
			}
		}
	}
}