namespace Messenger
{
    public class InMemoryMessageRepository : IMessageRepository
    {
        private readonly object locker = new object();

        public readonly Dictionary<Guid, Message> messagesById = new();

        public async Task PostMessage(Message message, Guid userId, IAccountRepository accountRepo, IChannelRepository channelRepo)
        {
            Channel channel = await channelRepo.GetChannel(message.ChannelId, userId, accountRepo);
            Account user = await accountRepo.GetAccount(userId);
            lock (locker)
            {
                if (channel.Users.Contains(user.Id) || user.isAdmin)
                {
                    messagesById.Add(message.Id, message);
                }
                else
                {
                    throw new Exception("User has no permission to post in this channel.");
                }
            }
        }

        public async Task<Message> GetMessage(Guid Id, Guid userId, IAccountRepository accountRepo, IChannelRepository channelRepo)
        {
            Message msg = messagesById[Id];
            Channel channel = await channelRepo.GetChannel(msg.ChannelId, userId, accountRepo);
            Account user = await accountRepo.GetAccount(userId);
            lock (locker)
            {
                if (channel.Users.Contains(user.Id) || user.isAdmin)
                {
                    try
                    {
                        return messagesById[Id];
                    }
                    catch (KeyNotFoundException)
                    {
                        throw new Exception($"Unable to find message with Id {Id}");
                    }
                }
                else
                {
                    throw new Exception("User has no permission to view messages in this channel.");
                }
            }
        }

        public async Task<Message[]> GetAllMessages(Guid userId, IAccountRepository accountRepo, IChannelRepository channelRepo)
        {
            Account user = await accountRepo.GetAccount(userId);
            lock (locker)
            {
                if (user.isAdmin)
                {
                    return messagesById.Values.ToArray();
                }
                else
                {
                    throw new Exception("User has no permission to view all Messages (Admin only request).");
                }
            }
        }

        public async Task<Message[]> GetAllMessagesInChannel(Guid channelId, Guid userId, IAccountRepository accountRepo, IChannelRepository channelRepo)
        {
            Channel channel = await channelRepo.GetChannel(channelId, userId, accountRepo);
            Account user = await accountRepo.GetAccount(userId);
            lock (locker)
            {
                if (channel.Users.Contains(user.Id) || user.isAdmin)
                {
                    return messagesById
                        .Where((x) => x.Value.ChannelId == channelId).ToDictionary().Values.ToArray();
                }
                else
                {
                    throw new Exception("User has no permission to view messages in this channel.");
                }
            }
        }
    }
}