using Messenger;

namespace MessengerInterfaces;

public class CleanUpService(IMessengerService messengerService, Logger logger)
{
	private DateTime lastCleanUpInMemory { get; set; } = DateTime.MinValue;

	public async Task<bool> RunCleanUpIfTimeLimitIsExceeded(TimeSpan? customFrequency = null)
	{
		TimeSpan frequency = customFrequency ?? CactusConstants.CleanUpFrequency;
		
		logger.Log("Checking if cleanup is needed", "CleanUp", "grey");

		if (DateTime.UtcNow - lastCleanUpInMemory < frequency)
		{
			logger.Log($"Skipping cleanup, last cleanup was at {lastCleanUpInMemory}", "CleanUp", "yellow");
			return false;
		}

		DateTime lastCleanUp;

		if (lastCleanUpInMemory == DateTime.MinValue)
		{
			logger.Log("No Clean Up Data in memory, fetching from database", "CleanUp", "yellow");
			CleanUpData data;

			try
			{
				data = await messengerService.GetCleanUpData();
			}
			catch (KeyNotFoundException)
			{
				data = new CleanUpData();
			}

			lastCleanUp = data.LastCleanUp;
			lastCleanUpInMemory = lastCleanUp;
		}
		else
		{
			lastCleanUp = lastCleanUpInMemory;
		}

		logger.Log($"Last cleanup was at {lastCleanUp}", "CleanUp", "grey");

		if (DateTime.UtcNow - lastCleanUp < frequency)
		{
			logger.Log("Skipping cleanup", "CleanUp", "yellow");
			return false;
		}

		await RunCleanUp();
		lastCleanUpInMemory = DateTime.UtcNow;
		await messengerService.SaveCleanUpData(new CleanUpData { LastCleanUp = lastCleanUpInMemory });
		return true;
	}

	public async Task RunCleanUp()
	{
		logger.Log("Running cleanup", "CleanUp", "yellow");
		List<Account> accounts = await messengerService.GetAllAccounts();

		foreach (Account account in accounts.Where(account => account.IsDemo))
		{
			if (DateTime.UtcNow - account.CreationDate <= CactusConstants.DemoAccountLifetime)
			{
				continue;
			}

			List<MessageDTO_Output> messages = await messengerService.GetMessagesByAccount(account.Id);

			foreach (MessageDTO_Output message in messages)
			{
				await messengerService.DeleteMessage(message.Id);
			}

			await messengerService.DeleteAccount(account.Id);
		}

		CleanUpData data;

		try
		{
			data = await messengerService.GetCleanUpData();
		}
		catch (KeyNotFoundException)
		{
			data = new CleanUpData();
		}

		data.LastCleanUp = DateTime.UtcNow;
		await messengerService.SaveCleanUpData(data);

		logger.Log("Cleanup finished", "CleanUp", "lime");
	}
}