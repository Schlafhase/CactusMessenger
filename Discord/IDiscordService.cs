using MessengerInterfaces;

namespace Discord;

public interface IDiscordService
{
	Task Run();
	Task SendCactusMessage(MessageDTO_Output msg);
}