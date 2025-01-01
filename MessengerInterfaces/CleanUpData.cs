using Messenger;
using Newtonsoft.Json;

namespace MessengerInterfaces;

public class CleanUpData : ICosmosObject
{
	[JsonProperty("id")] public Guid Id => CactusConstants.CleanUpDataId;
	public string Type  => "cleanUpData";
	
	public DateTime LastCleanUp { get; set; }
}