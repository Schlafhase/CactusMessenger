using CactusFrontEnd.Utils;
using Messenger;
using Microsoft.Azure.Cosmos;

namespace CactusFrontEnd.Cosmos;

public class CosmosAccountRepository(CosmosClient client)
	: CosmosRepositoryBase<Account>(client, "account");