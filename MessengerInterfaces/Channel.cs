using MessengerInterfaces;
using Newtonsoft.Json;

namespace Messenger;

public class Channel : ICosmosObject, IInMemoryObject
{
	public Channel(HashSet<Guid> users, Guid id, string name)
	{
		Users    = users;
		Id       = id;
		Name     = name;
		LastMessage = DateTime.UtcNow;
		LastRead = [];
	}

	public HashSet<Guid>              Users       { get; private set; }
	public string                     Name        { get; private set; }
	public DateTime                   LastMessage { get; set; }
	public Dictionary<Guid, DateTime> LastRead    { get; set; }

	[JsonProperty("id")] public Guid Id { get; private set; }

	public string Type => "channel";

	[JsonConstructor]
	public Channel(DateTime? LastMessage, Dictionary<Guid, DateTime>? LastRead)
	{
		this.LastMessage = LastMessage ?? DateTime.MinValue;
		this.LastRead    = LastRead    ?? [];
	}
}