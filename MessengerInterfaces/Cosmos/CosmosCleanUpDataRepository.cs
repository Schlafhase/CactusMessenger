using Microsoft.Azure.Cosmos;

namespace MessengerInterfaces;

public class CosmosCleanUpDataRepository(CosmosClient client)
	: CosmosRepositoryBase<CleanUpData>(client, "cleanUpData");