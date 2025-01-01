using MessengerInterfaces;
using Microsoft.Azure.Cosmos;

namespace CactusFrontEnd.Cosmos;

public class CosmosCleanUpDataRepository(CosmosClient client)
	: CosmosRepositoryBase<CleanUpData>(client, "cleanUpData");