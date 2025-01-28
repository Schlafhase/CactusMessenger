using MessengerInterfaces;
using Microsoft.Azure.Cosmos;

namespace CactusPay;

public class PaymentRepo(CosmosClient client) : CosmosRepositoryBase<PaymentManager>(client, "paymentManager") { }