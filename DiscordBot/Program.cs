using MessengerInterfaces;
using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using Message = Messenger.Message;

namespace DiscordBot;

public static class Program
{
	public static async Task Main(string[] args)
	{
		await discordService.Run();
		await discordService.SendCactusMessage(new MessageDTO_Output(
			                                       new Message(Guid.Empty, "Hello, World!", new DateTime(4200, 9, 6), Guid.Empty,
			                                                   Guid.Empty), "EVIL Lenus",true,
			                                       "Secret Testing Channle"));
	}
}