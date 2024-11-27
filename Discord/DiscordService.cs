using MessengerInterfaces;
using NetCord;
using NetCord.Gateway;
using NetCord.Rest;

namespace Discord;

public class DiscordService(string token)
{
	public  ulong         GuildId         { get; set; } = 1308518069554905120;
	public  ulong         GlobalChannelId { get; set; } = 1308519282975576098;
	private GatewayClient _client = new(new BotToken(token));

	public async Task Run()
	{
		_client.Log += message =>
		              {
			              Console.WriteLine(message);
			              return default;
		              };
		
		await _client.StartAsync();
	}

	public async Task SendCactusMessage(MessageDTO_Output msg)
	{
		MessageProperties message = createMessageFromCactusMessage<MessageProperties>(msg);
		await _client.Rest.SendMessageAsync(GlobalChannelId, message);
	}
	
	private static T createMessageFromCactusMessage<T>(MessageDTO_Output msg) where T : IMessageProperties, new()
	{
		EmbedProperties embed = new()
		                        {
			                        Author = new EmbedAuthorProperties
			                                 {
				                                 Name = $"Cactus Messenger - {msg.ChannelName}",
				                                 Url =
					                                 $"https://cactusmessenger.azurewebsites.net/channel/{msg.ChannelId}"
			                                 },
			                        Title       = msg.AuthorName,
			                        Description = msg.Content,
			                        Footer = new EmbedFooterProperties
			                                 {
				                                 Text = "https://messenger.schlafhase.uk",
				                                 IconUrl = "https://cactusmessenger.azurewebsites.net/Images/cactuslogo.png"
			                                 },
			                        Timestamp = msg.DateTime,
			                        Color = new Color(0x8B008B)
		                        };
		
		return new T
		       {
			       Embeds = [embed]
		       };
	}
}