using Acornbrot.LocalDB.Interfaces;
using MessengerInterfaces;
using MessengerInterfaces.Local;
using Newtonsoft.Json;

namespace MessengerInterfaces;

/// <summary>
///     Encapsulates a message.
/// </summary>
public class Message : ICosmosObject, ILocalObject
{
	[System.Text.Json.Serialization.JsonConstructor]
	public Message(Guid id, string content, DateTime dateTime, Guid authorId, Guid channelId)
	{
		Content = content;
		DateTime = dateTime;
		AuthorId = authorId;
		Id = id;
		ChannelId = channelId;
	}

	public string Content { get; private set; }
	public DateTime DateTime { get; private set; }
	public Guid AuthorId { get; private set; }
	public Guid ChannelId { get; private set; }

	[JsonProperty("id")] public Guid Id { get; private set; }

	public string Type => "message";
}