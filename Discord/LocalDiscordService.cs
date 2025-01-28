using MessengerInterfaces;

namespace Discord;

public class LocalDiscordService : IDiscordService
{
	public Task Run()
	{
		return Task.CompletedTask;
	}

	public Task SendCactusMessage(MessageDTO_Output msg)
	{
		return Task.CompletedTask;
	}
}