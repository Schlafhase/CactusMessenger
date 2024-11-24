using Messenger;
using MessengerInterfaces;
using Newtonsoft.Json;

namespace CactusPay;

public class PaymentManager : ICosmosObject
{
	public                      Dictionary<Guid, int> OpenTransactions { get; private set; }
	[JsonProperty("id")] public Guid                  Id               => CactusConstants.PaymentManagerId;
	public                      string                Type             => "paymentManager";
}