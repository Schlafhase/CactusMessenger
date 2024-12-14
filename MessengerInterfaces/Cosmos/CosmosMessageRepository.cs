using CactusFrontEnd.Utils;
using Messenger;
using Microsoft.Azure.Cosmos;

namespace CactusFrontEnd.Cosmos;

public class CosmosMessageRepository(CosmosClient client)
	: CosmosRepositoryBase<Message>(client, "message");