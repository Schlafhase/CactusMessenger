using Acornbrot.LocalDB.Interfaces;
using MessengerInterfaces;
using MessengerInterfaces.Local;
using Newtonsoft.Json;

namespace MessengerInterfaces;

public class CleanUpData : ICosmosObject, ILocalObject
{
	[JsonProperty("id")] public Guid Id => CactusConstants.CleanUpDataId;
	public string Type  => "cleanUpData";
	
	public DateTime LastCleanUp { get; set; }
}