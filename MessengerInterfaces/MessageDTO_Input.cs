using System.Text.Json.Serialization;

namespace Messenger;

/// <summary>
///     Encapsulates a message.
/// </summary>
public class MessageDTO_Input
{
	[JsonConstructor]
	public MessageDTO_Input(string content)
	{
		Content = content;
	}

	public string Content { get; }

	public Message ToMessage(Guid userId, Guid channelId)
	{
		return new Message(Guid.NewGuid(), Content, DateTime.UtcNow, userId, channelId);
	}
}