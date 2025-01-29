using Acornbrot.LocalDB.Interfaces;
using MessengerInterfaces;
using MessengerInterfaces.Local;
using Newtonsoft.Json;

namespace CactusPay;

public class PaymentManager : ICosmosObject, ILocalObject
{
	public Dictionary<Guid, int> OpenTransactions { get; private set; }
	[JsonProperty("id")] public Guid Id => CactusConstants.PaymentManagerId;
	public string Type => "paymentManager";
}