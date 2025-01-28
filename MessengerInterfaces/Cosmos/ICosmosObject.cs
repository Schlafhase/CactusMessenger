using Newtonsoft.Json;

namespace MessengerInterfaces;

public interface ICosmosObject
{
	[JsonProperty("id")] Guid Id { get; }

	string Type { get; }
}