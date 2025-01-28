using System.Text.Json.Serialization;
using MessengerInterfaces;

namespace MessengerInterfaces;

[method: JsonConstructor]
public class MessageDTO_Output(Message msg, string authorName, bool authorIsAdmin, string channelName)
{
	public string Content { get; } = msg.Content;
	public DateTime DateTime { get; } = msg.DateTime;
	public Guid AuthorId { get; } = msg.AuthorId;
	public string AuthorName { get; } = authorName;
	public bool AuthorIsAdmin { get; } = authorIsAdmin;
	public Guid ChannelId { get; } = msg.ChannelId;
	public string ChannelName { get; } = channelName;
	public Guid Id { get; } = msg.Id;
}