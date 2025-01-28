using System.Linq.Expressions;

namespace MessengerInterfaces;

public interface IRepository<T>
{
	Task CreateNew(T entity);
	Task Replace(Guid id, T entity);

	/// <summary>
	///     Gets the object with the given <paramref name="id" />. Returns null if not found.
	/// </summary>
	Task<T> GetById(Guid id);

	Task<List<T>> GetAll();

	IQueryable<T> GetQueryable();

	Task<List<TElement>> ToListAsync<TElement>(IQueryable<TElement> query);
	Task DeleteItemsWithFilter(Expression<Func<T, bool>> filter);
	Task DeleteItem(Guid id);
	Task UpdateItemVoid(Guid id, Action<T> update);
}