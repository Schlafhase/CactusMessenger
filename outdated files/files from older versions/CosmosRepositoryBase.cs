using CactusFrontEnd.Cosmos.utils;
using Messenger;
using Microsoft.Azure.Cosmos;
using System.Net;

namespace outdated
{
	public abstract class CosmosRepositoryBase<T> where T : ICosmosObject
	{
		CosmosClient client { get; }
		Container container { get; }
		PartitionKey partitionKey;
		protected CosmosRepositoryBase(CosmosClient client)
		{
			this.client = client;
			container = client.GetContainer("cactus-messenger", "cactus-messenger");
			partitionKey = new("id");
		}

		protected async Task Create(T entity)
		{
			await container.CreateItemAsync<T>(entity, partitionKey);
		}

		protected async Task Replace(T entity, string id)
		{
			await container.ReplaceItemAsync<T>(entity, id, partitionKey);
		}

		protected async Task<T> GetById(Guid id)
		{
			var q = container.GetItemLinqQueryable<T>()
				.Where(item => item.Id == id);
			List<T> result = await this.GetQuery(q);
			return result.First();
		}

		protected async Task<List<T>> GetByType(string type)
		{
			var q = container.GetItemLinqQueryable<T>()
				.Where(item => item.Type == type);
			List<T> result = await this.GetQuery(q);
			return result;
		}

		protected async Task<List<T>> GetQuery(IQueryable<T> q)
		{
			IAsyncEnumerable<T> response = Utils.ExecuteQuery<T>(q);
			List<T> result = await Utils.ToListAsync<T>(response);
			return result;
		}
	}
}
