using Acornbrot.LocalDB.Interfaces;
using MessengerInterfaces;
using MessengerInterfaces.Local;
using Newtonsoft.Json;

namespace MessengerInterfaces;

public class Channel : ICosmosObject, ILocalObject
{
	public Channel(HashSet<Guid> users, Guid id, string name, Guid authorId)
	{
		Users = users ?? throw new ArgumentNullException(nameof(users));
		Id = id;
		Name = name ?? throw new ArgumentNullException(nameof(name));
		LastMessage = DateTime.UtcNow;
		LastRead = [];
		AuthorId = authorId;
	}

	[JsonConstructor]
	public Channel(DateTime? LastMessage, Dictionary<Guid, DateTime>? LastRead)
	{
		this.LastMessage = LastMessage ?? DateTime.MinValue;
		this.LastRead = LastRead ?? [];
	}

	public HashSet<Guid> Users { get; set; }
	public string Name { get; set; }
	public Guid AuthorId { get; set; }
	public DateTime LastMessage { get; set; }
	public Dictionary<Guid, DateTime> LastRead { get; set; }

	[JsonProperty("id")] public Guid Id { get; set; }

	public string Type => "channel";
}