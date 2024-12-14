using System.Linq.Expressions;
using System.Net;
using CactusFrontEnd.Exceptions;
using CactusFrontEnd.Utils;
using Messenger;
using MessengerInterfaces;
using Microsoft.Azure.Cosmos;

namespace CactusFrontEnd.Cosmos;

public abstract class CosmosRepositoryBase<T>(CosmosClient client, string type) : IRepository<T>
	where T : class, ICosmosObject
{
	private readonly AsyncLocker asyncLocker = new();
	private readonly CosmosClient client = client;
	private readonly Container container = client.GetContainer("cactus-messenger", "cactus-messenger");

	public async Task CreateNew(T entity)
	{
		try
		{
			ItemResponse<T> response =
				await container.CreateItemAsync<T>(entity, new PartitionKey(entity.Id.ToString()));

			if (response.StatusCode is HttpStatusCode.Conflict)
			{
				throw new StatusCodeException(HttpStatusCode.Conflict);
			}
		}
		catch (CosmosException ex) when (ex.StatusCode is HttpStatusCode.Conflict)
		{
			throw new StatusCodeException(HttpStatusCode.Conflict);
		}
	}

	public async Task<List<T>> GetAll()
	{
		IQueryable<T> q = GetQueryable();
		return await ToListAsync(q);
	}

	public async Task<T?> GetById(Guid id)
	{
		try
		{
			IQueryable<T> q = GetQueryable()
				.Where(item => item.Id == id);
			List<T> result = await ToListAsync(q);

			//ItemResponse<T> result = await container.ReadItemAsync<T>(id.ToString(), new PartitionKey(id.ToString()));
			if (result.Count != 1)
			{
				return null;
			}

			return result.First();
		}
		catch (CosmosException e) when (e.StatusCode is HttpStatusCode.NotFound)
		{
			return null;
		}
	}

	public IQueryable<T> GetQueryable()
	{
		return container.GetItemLinqQueryable<T>()
						.Where(item => item.Type == type);
	}

	public async Task DeleteItemsWithFilter(Expression<Func<T, bool>> filter)
	{
		IQueryable<Guid> query = GetQueryable()
								 .Where(filter)
								 .Select(item => item.Id);
		List<Guid> ids = await ToListAsync(query);

		await Task.WhenAll(ids
							   .Select(id => DeleteItem(id)));
	}

	public async Task DeleteItem(Guid id)
	{
		using IDisposable _ = await asyncLocker.Enter();
		await container.DeleteItemAsync<T>(id.ToString(), new PartitionKey(id.ToString()));
	}

	public async Task Replace(Guid id, T entity)
	{
		using IDisposable _ = await asyncLocker.Enter();
		await container.ReplaceItemAsync<T>(entity, id.ToString(), new PartitionKey(id.ToString()));
	}

	public async Task<List<TElement>> ToListAsync<TElement>(IQueryable<TElement> query)
	{
		IAsyncEnumerable<TElement> response = Utils.Utils.ExecuteQuery(query);
		List<TElement> result = await Utils.Utils.ToListAsync(response);
		return result;
	}

	public async Task UpdateItemVoid(Guid id, Action<T> update)
	{
		using IDisposable _ = await asyncLocker.Enter();
		T entity = await GetById(id);
		update(entity);
		await Replace(id, entity);
	}
}