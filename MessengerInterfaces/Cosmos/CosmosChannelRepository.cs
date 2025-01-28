using Microsoft.Azure.Cosmos;

namespace MessengerInterfaces;

public class CosmosChannelRepository(CosmosClient client)
	: CosmosRepositoryBase<Channel>(client, "channel");