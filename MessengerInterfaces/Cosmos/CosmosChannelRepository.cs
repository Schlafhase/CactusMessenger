using Messenger;
using Microsoft.Azure.Cosmos;

namespace CactusFrontEnd.Cosmos;

public class CosmosChannelRepository(CosmosClient client)
	: CosmosRepositoryBase<Channel>(client, "channel");