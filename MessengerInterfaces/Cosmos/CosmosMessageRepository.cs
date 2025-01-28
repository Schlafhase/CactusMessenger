using Microsoft.Azure.Cosmos;

namespace MessengerInterfaces;

public class CosmosMessageRepository(CosmosClient client)
	: CosmosRepositoryBase<Message>(client, "message");