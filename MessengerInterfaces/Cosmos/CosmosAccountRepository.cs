using Microsoft.Azure.Cosmos;

namespace MessengerInterfaces;

public class CosmosAccountRepository(CosmosClient client)
	: CosmosRepositoryBase<Account>(client, "account");