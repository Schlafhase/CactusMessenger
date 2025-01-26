using CactusFrontEnd.Events;
using CactusFrontEnd.Exceptions;
using CactusFrontEnd.Utils;
using Messenger;
using MessengerInterfaces;

namespace CactusFrontEnd.Cosmos;

public class MessengerService(
	IRepository<Account> accountRepo,
	IRepository<Channel> channelRepo,
	IRepository<Message> messageRepo,
	IRepository<CleanUpData> cleanUpDataRepo,
	EventService eventService,
	Logger logger)
	: IMessengerService
{
	private readonly AsyncLocker asyncLocker = new();
	public event Action<ChannelDTO_Output>? OnMessage;

	public async Task InitializeAsync() { }

	private async Task<MessageDTO_Output> getMessage(Guid Id)
	{
		Message msg = await messageRepo.GetById(Id);
		Account author = await getAccount(msg.AuthorId);
		ChannelDTO_Output channel = await getChannel(msg.ChannelId);

		try
		{
			MessageDTO_Output msgDTO = new(msg, author.UserName, author.IsAdmin, channel.Name);
			return msgDTO;
		}
		catch (KeyNotFoundException)
		{
			throw new KeyNotFoundException($"Unable to find message with Id {Id}");
		}
	}

	private async Task deleteChannel(Guid channelId)
	{
		//delete all messages in channel
		await messageRepo.DeleteItemsWithFilter(msg => msg.ChannelId == channelId);
		//delete channel
		await channelRepo.DeleteItem(channelId);
		logger.Log($"Deleted channel with Id {channelId}");
	}

	private async Task removeUserFromChannel(Guid channelId, Guid accountId)
	{
		ChannelDTO_Output channelDTO = await getChannel(channelId);
		channelDTO.Users.Remove(accountId);

		if (channelDTO.Users.Count == 0)
		{
			await deleteChannel(channelId);
		}
		else
		{
			Channel channel = new(channelDTO.Users, channelId, channelDTO.Name, channelDTO.AuthorId)
			{
				LastMessage = channelDTO.LastMessage,
				LastRead = channelDTO.LastRead
			};
			await channelRepo.Replace(channelId, channel);
		}

		;
		logger.Log($"Removed user {accountId} from channel {channelId}");
		eventService.ChannelsHaveChanged();
	}

	private async Task<ChannelDTO_Output[]> getAllChannels()
	{
		IQueryable<Channel> query = channelRepo.GetQueryable();
		List<Channel> channels = await channelRepo.ToListAsync(query);
		return await convertChannelsToDTOs(channels);
	}

	private async Task<ChannelDTO_Output> getChannel(Guid channelId)
	{
		Channel channel;

		try
		{
			channel = await channelRepo.GetById(channelId);
		}
		catch (KeyNotFoundException)
		{
			throw new KeyNotFoundException($"Unable to find channel with Id {channelId}");
		}

		if (channel is null)
		{
			throw new KeyNotFoundException($"Unable to find channel with Id {channelId}");
		}


		ChannelDTO_Output channelDTO = await createChannelDTO_OutputFromChannel(channel);
		return channelDTO;
	}

	public async Task<Guid> CreateAccount(string username, string password, string? email = null, bool demo = false)
	{
		using IDisposable _ = await asyncLocker.Enter();

		try
		{
			await getAccountByUsername(username);
		}
		catch (KeyNotFoundException)
		{
			Guid userId = Guid.NewGuid();
			string passwordHash = Utils.Utils.GetStringSha256Hash(password + userId);
			await accountRepo.CreateNew(new Account(username, passwordHash, userId, email, demo));
			logger.Log($"Account created with username {username}");
			return userId;
		}

		throw new UsernameExistsException();
	}

	private async Task<Account> getAccountByUsername(string username)
	{
		IQueryable<Account> q = accountRepo.GetQueryable()
										   .Where(item => item.UserName == username);
		List<Account> result = await accountRepo.ToListAsync(q);

		if (result.Count == 1)
		{
			return result[0];
		}

		throw new KeyNotFoundException("User not found");
	}

	private async Task<List<Account>> getExpiredDemoAccounts()
	{
		// TODO: Query is not returning results.
		DateTime expiredCreationDate = DateTime.UtcNow - CactusConstants.DemoAccountLifetime;
		IQueryable<Account> q = accountRepo.GetQueryable()
										   .Where(item => item.IsDemo && item.CreationDate < expiredCreationDate);
		List<Account> result = await accountRepo.ToListAsync(q);

		return result;
	}

	private async Task<Account> getAccount(Guid Id)
	{
		Account? acc = await accountRepo.GetById(Id);

		if (acc is null)
		{
			throw new KeyNotFoundException($"Unable to find Account with Id {Id}");
		}

		return acc;
	}

	private async Task<List<Account>> getAllAccounts()
	{
		IQueryable<Account> q = accountRepo.GetQueryable();
		List<Account> result = await accountRepo.ToListAsync(q);
		return result;
	}

	//other methods

	private async Task<MessageDTO_Output[]> convertMessagesToDTOs(List<Message> messages)
	{
		MessageDTO_Output[] DTOs = await Task.WhenAll(messages
														  .Select(async msg =>
														  {
															  try
															  {
																  Account author = await getAccount(msg.AuthorId);
																  ChannelDTO_Output channel =
																	  await getChannel(msg.ChannelId);
																  return new MessageDTO_Output(
																	  msg, author.UserName, author.IsAdmin,
																	  channel.Name);
															  }
															  catch (Exception)
															  {
																  return null!;
															  }
														  }));
		MessageDTO_Output[] DTOsNotNull = DTOs
										  .Where(x => x is not null)
										  .ToArray();
		return DTOsNotNull;
	}

	private async Task<ChannelDTO_Output[]> convertChannelsToDTOs(List<Channel> channels)
	{
		return await Task.WhenAll(channels
								  .Select(async channel =>
								  {
									  string[] userNames = await Task.WhenAll(channel.Users
																				  .Select(async userId =>
																				  {
																					  Account user =
																						  await getAccount(userId);
																					  return user.UserName;
																				  }));
									  return new ChannelDTO_Output(channel, userNames.ToHashSet());
								  })
								  .Where(x => x != null)
								  .ToArray());
	}

	private async Task<ChannelDTO_Output> createChannelDTO_OutputFromChannel(Channel channel)
	{
		string[] UserNames = await Task.WhenAll(channel.Users
													   .Select(async userId =>
													   {
														   if (userId == CactusConstants.EveryoneId)
														   {
															   return "Everyone";
														   }

														   Account user = await getAccount(userId);
														   return user.UserName;
													   }));
		return new ChannelDTO_Output(channel, UserNames.ToHashSet());
	}

	#region Message related methods

	public async Task<MessageDTO_Output> GetMessage(Guid Id) => await getMessage(Id);

	public async Task PostMessage(Message message)
	{
		using IDisposable _ = await asyncLocker.Enter();
		Account author;

		try
		{
			author = await getAccount(message.AuthorId);

			if (author.IsDemo)
			{
				if (author.TotalMessagesSent >= CactusConstants.DemoAccountMaxMessageCount)
				{
					throw new InvalidOperationException("Demo account has reached the maximum message count");
				}
			}
		}
		catch (KeyNotFoundException)
		{
			throw new KeyNotFoundException($"Unable to find author {message.AuthorId}");
		}

		ChannelDTO_Output channel = await getChannel(message.ChannelId);
		await messageRepo.CreateNew(message);
		channel.LastMessage = message.DateTime;
		await updateLastMessageTime(channel.Id, message.DateTime);
		await updateTotalMessagesSent(message.AuthorId, author.TotalMessagesSent + 1);
		logger.Log($"Message posted in channel {channel.Name}");
		OnMessage?.Invoke(channel);
	}

	public async Task DeleteMessage(Guid id)
	{
		using IDisposable _ = await asyncLocker.Enter();
		MessageDTO_Output msg = await getMessage(id);

		ChannelDTO_Output channel = await getChannel(msg.ChannelId);
		//delete message
		await messageRepo.DeleteItem(id);
		logger.Log($"Message deleted in channel {channel.Name}");
		OnMessage?.Invoke(channel);
	}

	public async Task DeleteAllMessages()
	{
		using IDisposable _ = await asyncLocker.Enter();
		await messageRepo.DeleteItemsWithFilter(item => true);
	}

	public async Task<MessageDTO_Output[]> GetAllMessages()
	{
		using IDisposable _ = await asyncLocker.Enter();

		List<Message> messages = await messageRepo.GetAll();
		return await convertMessagesToDTOs(messages);
	}

	public async Task<MessageDTO_Output[]> GetAllMessagesInChannel(Guid channelId)
	{
		using IDisposable _ = await asyncLocker.Enter();
		IQueryable<Message> query = messageRepo.GetQueryable()
											   .Where(msg => msg.ChannelId == channelId);

		List<Message> messages = await messageRepo.ToListAsync(query);
		return await convertMessagesToDTOs(messages);
	}

	public async Task<List<MessageDTO_Output>> GetMessagesByAccount(Guid accountId)
	{
		using IDisposable _ = await asyncLocker.Enter();
		IQueryable<Message> query = messageRepo.GetQueryable()
											   .Where(msg => msg.AuthorId == accountId);
		List<Message> messages = await messageRepo.ToListAsync(query);
		return (await convertMessagesToDTOs(messages)).ToList();
	}

	#endregion

	#region Channel related methods

	public async Task<Guid> CreateChannel(HashSet<Guid> userIds, string name, Guid authorId)
	{
		using IDisposable _ = await asyncLocker.Enter();

		Account author;

		try
		{
			author = await getAccount(authorId);

			if (author.IsDemo)
			{
				if (author.TotalMessagesSent >= CactusConstants.DemoAccountMaxMessageCount)
				{
					throw new InvalidOperationException("Demo account has reached the maximum channel count");
				}
			}
		}
		catch (KeyNotFoundException)
		{
			throw new KeyNotFoundException($"Unable to find author {authorId}");
		}
		
		Guid channelId = Guid.NewGuid();
		await channelRepo.CreateNew(new Channel(userIds, channelId, name, authorId));
		await updateTotalChannelsCreated(authorId, author.TotalChannelsCreated + 1);
		logger.Log($"Channel created with name {name}");
		eventService.ChannelsHaveChanged();
		return channelId;
	}

	public async Task DeleteChannel(Guid channelId)
	{
		using IDisposable _ = await asyncLocker.Enter();
		await deleteChannel(channelId);
		eventService.ChannelsHaveChanged();
	}

	public async Task RemoveUserFromChannel(Guid channelId, Guid accountId)
	{
		using IDisposable _ = await asyncLocker.Enter();
		await removeUserFromChannel(channelId, accountId);
	}

	public async Task UpdateLastMessageTime(Guid channelId, DateTime time)
	{
		using IDisposable _ = await asyncLocker.Enter();
		await updateLastMessageTime(channelId, time);
	}

	private async Task updateLastMessageTime(Guid channelId, DateTime time)
	{
		Channel channel;

		try
		{
			channel = await channelRepo.GetById(channelId);
		}
		catch (KeyNotFoundException)
		{
			throw new KeyNotFoundException($"Unable to find channel with Id {channelId}");
		}

		channel.LastMessage = time;
		await channelRepo.Replace(channelId, channel);
		logger.Log($"Updated last message time in channel {channelId}");
	}

	public async Task UpdateLastRead(Guid channelId, Guid accountId, DateTime time)
	{
		using IDisposable _ = await asyncLocker.Enter();
		await updateLastRead(channelId, accountId, time);
	}

	private async Task updateLastRead(Guid channelId, Guid accountId, DateTime time)
	{
		Channel channel;

		try
		{
			channel = await channelRepo.GetById(channelId);
		}
		catch (KeyNotFoundException)
		{
			throw new KeyNotFoundException($"Unable to find channel with Id {channelId}");
		}

		channel.LastRead[accountId] = time;
		await channelRepo.Replace(channelId, channel);
		logger.Log($"Updated last read time for user {accountId} in channel {channelId}");
	}

	public async Task<ChannelDTO_Output> GetChannel(Guid channelId)
	{
		using IDisposable _ = await asyncLocker.Enter();
		return await getChannel(channelId);
	}

	public async Task<ChannelDTO_Output[]> GetChannelsWithUser(Guid accountId)
	{
		using IDisposable _ = await asyncLocker.Enter();
		IQueryable<Channel> query = channelRepo.GetQueryable()
											   .Where(channel => channel.Users.Contains(accountId));
		List<Channel> channels = await channelRepo.ToListAsync(query);
		return await convertChannelsToDTOs(channels);
	}
	
	public async Task<ChannelDTO_Output[]> GetChannelsFromAuthor(Guid authorId)
	{
		using IDisposable _ = await asyncLocker.Enter();
		IQueryable<Channel> query = channelRepo.GetQueryable()
											   .Where(channel => channel.AuthorId == authorId);
		List<Channel> channels = await channelRepo.ToListAsync(query);
		return await convertChannelsToDTOs(channels);
	}

	public async Task<ChannelDTO_Output[]> GetAllChannels()
	{
		using IDisposable _ = await asyncLocker.Enter();
		return await getAllChannels();
	}

	public async Task AddUserToChannel(Guid Id, Guid channelId)
	{
		using IDisposable _ = await asyncLocker.Enter();
		Channel channel;

		try
		{
			channel = await channelRepo.GetById(channelId);
		}
		catch (KeyNotFoundException)
		{
			throw new KeyNotFoundException($"Unable to find channel with Id {channelId}");
		}

		channel.Users.Add(Id);
		await channelRepo.Replace(channelId, channel);
		logger.Log($"Added user {Id} to channel {channelId}");
		eventService.ChannelsHaveChanged();
	}

	#endregion

	#region Account related methods

	public async Task<bool> LoginAccount(Guid id, string password)
	{
		using IDisposable _ = await asyncLocker.Enter();
		Account user = await getAccount(id);
		string passwordHash = Utils.Utils.GetStringSha256Hash(password + user.Id);
		logger.Log($"User {user.UserName} logged in");
		return passwordHash == user.PasswordHash;
	}

	public async Task DeleteAccount(Guid id)
	{
		if (id == CactusConstants.AdminId || id == CactusConstants.DeletedId)
		{
			throw new ArgumentException("This account can't be deleted.");
		}

		using IDisposable _ = await asyncLocker.Enter();
		//remove user from all channels
		IQueryable<Message> messageQuery = messageRepo.GetQueryable()
													  .Where(msg => msg.AuthorId == id);
		List<Message> messages = await messageRepo.ToListAsync(messageQuery);
		//change all authorids from user to deleted CactusConstants.DeletedId
		await Task.WhenAll(messages
							   .Select(msg =>
							   {
								   Message newMessage = new(msg.Id, msg.Content, msg.DateTime,
															CactusConstants.DeletedId, msg.ChannelId);
								   return messageRepo.Replace(msg.Id, newMessage);
							   }));
		IQueryable<Guid> channelQuery = channelRepo.GetQueryable()
												   .Where(channel => channel.Users.Contains(id))
												   .Select(channel => channel.Id);

		List<Guid> channelIds = await channelRepo.ToListAsync(channelQuery);
		await Task.WhenAll(channelIds
							   .Select(channelId => { return removeUserFromChannel(channelId, id); }));
		eventService.ChannelsHaveChanged();

		//delete account
		await accountRepo.DeleteItem(id);
		logger.Log($"Account with Id {id} deleted");
	}

	public async Task EditAccountAdmin(Guid id, bool giveAdmin)
	{
		using IDisposable _ = await asyncLocker.Enter();

		Account target = await getAccount(id);
		target.IsAdmin = giveAdmin;
		await accountRepo.Replace(id, target);
		logger.Log($"Account with Id {id} is now {(giveAdmin ? "an admin" : "not an admin")}");
	}

	public async Task EditAccountLock(Guid id, bool newState)
	{
		using IDisposable _ = await asyncLocker.Enter();

		Account target = await getAccount(id);
		target.Locked = newState;
		await accountRepo.Replace(id, target);
		logger.Log($"Account with Id {id} is now {(newState ? "locked" : "unlocked")}");
	}

	public async Task EditAccountEmail(Guid id, string email)
	{
		using IDisposable _ = await asyncLocker.Enter();

		Account target = await getAccount(id);
		target.Email = email;
		await accountRepo.Replace(id, target);
		logger.Log($"Added an email: {email} to account {target.UserName}");
	}

	public async Task ChangePW(Guid Id, string newPW)
	{
		using IDisposable _ = await asyncLocker.Enter();

		Account target = await getAccount(Id);
		string passwordHash = Utils.Utils.GetStringSha256Hash(newPW + Id);
		target.PasswordHash = passwordHash;
		await accountRepo.Replace(Id, target);
		logger.Log($"Password changed for account {target.UserName}");
	}

	public async Task ChangePWHash(Guid Id, string newPWHash)
	{
		using IDisposable _ = await asyncLocker.Enter();

		Account target = await getAccount(Id);
		target.PasswordHash = newPWHash;
		await accountRepo.Replace(Id, target);
		logger.Log($"Password changed for account {target.UserName}");
	}

	public async Task UpdateAccountBalance(Guid Id, float amount)
	{
		using IDisposable _ = await asyncLocker.Enter();
		Account account = await getAccount(Id);
		account.Balance = amount;
		await accountRepo.Replace(Id, account);
		logger.Log($"Updated balance for account {account.UserName}");
	}

	public async Task UpdateAccountLastLogin(Guid Id, DateTime date)
	{
		using IDisposable _ = await asyncLocker.Enter();
		Account account = await getAccount(Id);
		account.LastLogin = date;
		await accountRepo.Replace(Id, account);
		logger.Log($"Updated last login for account {account.UserName}");
	}

	public async Task UpdateAccountLoginStreak(Guid Id, int streak)
	{
		using IDisposable _ = await asyncLocker.Enter();
		Account account = await getAccount(Id);
		account.LoginStreak = streak;
		account.LastStreakChange = DateTime.UtcNow;
		await accountRepo.Replace(Id, account);
		logger.Log($"Updated login streak for account {account.UserName}. New streak: {streak}");
	}

	public async Task UpdateTotalMessagesSent(Guid Id, int amount)
	{
		using IDisposable _ = await asyncLocker.Enter();
		await updateTotalMessagesSent(Id, amount);
	}

	private async Task updateTotalMessagesSent(Guid Id, int amount)
	{
		Account account = await getAccount(Id);
		account.TotalMessagesSent = amount;
		await accountRepo.Replace(Id, account);
		logger.Log($"Updated total messages sent for account {account.UserName}. New total: {amount}");
	}
	
	public async Task UpdateTotalChannelsCreated(Guid Id, int amount)
	{
		using IDisposable _ = await asyncLocker.Enter();
		await updateTotalChannelsCreated(Id, amount);
	}
	
	private async Task updateTotalChannelsCreated(Guid Id, int amount)
	{
		Account account = await getAccount(Id);
		account.TotalChannelsCreated = amount;
		await accountRepo.Replace(Id, account);
		logger.Log($"Updated total channels created for account {account.UserName}. New total: {amount}");
	}

	public async Task<Account> GetAccount(Guid Id)
	{
		using IDisposable _ = await asyncLocker.Enter();
		return await getAccount(Id);
	}

	public async Task<Account> GetAccountByUsername(string username)
	{
		using IDisposable _ = await asyncLocker.Enter();
		return await getAccountByUsername(username);
	}

	public async Task<List<Account>> GetExpiredDemoAccounts()
	{
		using IDisposable _ = await asyncLocker.Enter();
		return await getExpiredDemoAccounts();
	}

	public async Task<List<Account>> GetAllAccounts()
	{
		using IDisposable _ = await asyncLocker.Enter();
		return await getAllAccounts();
	}

	#endregion

	#region Clean up methods

	public async Task<CleanUpData> GetCleanUpData()
	{
		CleanUpData? data = await cleanUpDataRepo.GetById(CactusConstants.CleanUpDataId);

		return data ?? throw new KeyNotFoundException("Clean up data not found");
	}

	public async Task SaveCleanUpData(CleanUpData data)
	{
		await cleanUpDataRepo.Replace(CactusConstants.CleanUpDataId, data);
	}

	#endregion
}