
using Microsoft.AspNetCore.Components;
using MessengerInterfaces;
using Messenger;

namespace outdated
{
    public class InMemoryChannelService : IChannelService
    {
        public readonly Dictionary<Guid, Channel> channels = new();
		private IAccountService accountRepo;

		public InMemoryChannelService(IAccountService accountRepo)
        {
            channels[CactusConstants.AdminId] = new Channel([CactusConstants.AdminId], CactusConstants.AdminId);
            channels[CactusConstants.DeletedId] = new Channel([CactusConstants.DeletedId], CactusConstants.DeletedId);
            this.accountRepo = accountRepo;
        }
        public async Task<Guid> CreateChannel(HashSet<Guid> userIds, Guid userId)
        {
            Account user = await accountRepo.GetAccount(userId);
            if (userIds.Contains(user.Id) || user.IsAdmin)
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

        public async Task<ChannelDTO_Output> GetChannel(Guid channelId, Guid userId)
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
            if (channel.Users.Contains(user.Id) || user.IsAdmin)
            {
                ChannelDTO_Output channelDTO = await ChannelDTO_Output.FromChannel(channel, accountRepo);
                return channelDTO;
            }
            else
            {
                throw new Exception("User has no permission to see the requested channel.");
            }
        }

		public async Task AddUserToChannel(Guid Id, Guid channelId, Guid userId)
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
			if (channel.Users.Contains(user.Id) || user.IsAdmin)
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