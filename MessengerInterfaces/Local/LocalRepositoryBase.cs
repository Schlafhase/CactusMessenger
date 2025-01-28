using System.Linq.Expressions;
using System.Text.Json;
using Acornbrot.LocalDB;
using Acornbrot.LocalDB.Interfaces;
using MessengerInterfaces.Utils;

namespace MessengerInterfaces.Local;

public class LocalRepositoryBase<T>(string root, string type) : IRepository<T> where T : IDbObject
{
	private readonly LocalDbClient _client = new(root);
	private readonly AsyncLocker _asyncLocker = new();
	
	public async Task CreateNew(T entity)
	{
		await _client.CreateAsync(entity);
	}

	public async Task Replace(Guid id, T entity)
	{
		using IDisposable _ = await _asyncLocker.Enter();
		await _client.ReplaceAsync(entity);
	}

	public async Task<T> GetById(Guid id)
	{
		return await _client.GetAsync<T>(id);
	}

	public async Task<List<T>> GetAll()
	{
		var q = GetQueryable();
		return await ToListAsync(q);
	}

	public IQueryable<T> GetQueryable()
	{
		return _client.GetItemLinqQueryable<T>()
					  .Where(item => item.Type == type);
	}

	public async Task<List<TElement>> ToListAsync<TElement>(IQueryable<TElement> query)
	{
		return _client.ToList(query);
	}

	public async Task DeleteItemsWithFilter(Expression<Func<T, bool>> filter)
	{
		IQueryable<Guid> query = GetQueryable()
								 .Where(filter)
								 .Select(item => item.Id);
		List<Guid> ids = await ToListAsync(query);

		await Task.WhenAll(ids
							   .Select(DeleteItem));
	}

	public async Task DeleteItem(Guid id)
	{
		using IDisposable _ = await _asyncLocker.Enter();
		await _client.DeleteAsync(id);
	}

	public async Task UpdateItemVoid(Guid id, Action<T> update)
	{
		using IDisposable _ = await _asyncLocker.Enter();
		T item = await GetById(id);
		update(item);
		await Replace(id, item);
	}
}