using MessengerInterfaces;
using System.Threading.Channels;
using Messenger;

namespace outdated
{
	public class InMemoryMessageService : IMessageService
	{
		public Action<ChannelDTO_Output> OnMessage {  get; set; }
		private readonly object locker = new object();

		public readonly Dictionary<Guid, Message> messagesById = new();
		private IAccountService accountRepo;
		private IChannelService channelRepo;

		public InMemoryMessageService(IAccountService accountRepo, IChannelService channelRepo)
		{
			this.accountRepo = accountRepo;
			this.channelRepo = channelRepo;
		}

		public async Task PostMessage(Message message, Guid userId)
		{
			ChannelDTO_Output channel = await channelRepo.GetChannel(message.ChannelId, userId);
			Account user = await accountRepo.GetAccount(userId);
			lock (locker)
			{
				if (channel.Users.Contains(user.Id) || user.IsAdmin)
				{
					messagesById.Add(message.Id, message);
				}
				else
				{
					throw new Exception("User has no permission to post in this channel.");
				}
			}
			OnMessage.Invoke(channel);
		}

		public async Task<MessageDTO_Output> GetMessage(Guid Id, Guid userId)
		{
			Message msg = messagesById[Id];
			Account author = await accountRepo.GetAccount(msg.AuthorId);
			ChannelDTO_Output channel = await channelRepo.GetChannel(msg.ChannelId, userId);
			Account user = await accountRepo.GetAccount(userId);
			lock (locker)
			{
				if (channel.Users.Contains(user.Id) || user.IsAdmin)
				{
					try
					{
						MessageDTO_Output msgDTO = new(msg, author.UserName);
						return msgDTO;
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


		public async Task<MessageDTO_Output[]> GetAllMessages(Guid userId)
		{
			Account user = await accountRepo.GetAccount(userId);
			Message[] messages;

			lock (locker)
			{
				messages = messagesById.Values.ToArray();
			}

			if (user.IsAdmin)
			{
				return await convertToDtos(accountRepo, messages);
			}
			else
			{
				throw new Exception("User has no permission to view all Messages (Admin only request).");
			}
		}

		public async Task<MessageDTO_Output[]> GetAllMessagesInChannel(Guid channelId, Guid userId)
		{
			Account user = await accountRepo.GetAccount(userId);
			ChannelDTO_Output channel = await channelRepo.GetChannel(channelId, userId);
			Message[] messages;

			lock (locker)
			{
				messages = messagesById.Values
						.Where(msg => msg.ChannelId == channelId)
						.ToArray();
			}


			if (channel.Users.Contains(user.Id) || user.IsAdmin)
			{
				return await convertToDtos(accountRepo, messages);
			}
			else
			{
				throw new Exception("User has no permission to view messages in this channel.");
			}

		}

		private static async Task<MessageDTO_Output[]> convertToDtos(IAccountService accountRepo, Message[] messages)
		{
			return await Task.WhenAll(messages
							.Select(async (msg) =>
							{
								try
								{
									Account author = await accountRepo.GetAccount(msg.AuthorId);
									return new MessageDTO_Output(msg, author.UserName);
								}
								catch (Exception)
								{
									return null!;
								}
							})
							.Where(x => x != null)
							.ToArray());
		}

		//public async Task<Message[]> GetAllMessagesInChannel(Guid channelId, Guid userId, IAccountRepository accountRepo, IChannelRepository channelRepo)
		//{
		//    Channel channel = await channelRepo.GetChannel(channelId, userId, accountRepo);
		//    Account user = await accountRepo.GetAccount(userId);
		//    lock (locker)
		//    {
		//        if (channel.Users.Contains(user.Id) || user.isAdmin)
		//        {
		//            return messagesById
		//                .Where((x) => x.Value.ChannelId == channelId).ToDictionary().Values.ToArray();
		//        }
		//        else
		//        {
		//            throw new Exception("User has no permission to view messages in this channel.");
		//        }
		//    }
		//}
	}
}