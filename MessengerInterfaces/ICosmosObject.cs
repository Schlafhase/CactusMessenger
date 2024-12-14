using Newtonsoft.Json;

namespace Messenger;

public interface ICosmosObject
{
	[JsonProperty("id")] Guid Id { get; }

	string Type { get; }
}