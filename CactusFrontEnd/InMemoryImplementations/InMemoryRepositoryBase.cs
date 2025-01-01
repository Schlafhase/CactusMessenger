using MessengerInterfaces;

namespace CactusFrontEnd.InMemoryImplementations
{
	public abstract class InMemoryRepositoryBase<T> : IRepository<T> where T : IInMemoryObject
	{
		private readonly Dictionary<Guid, T> data = new();

		public async Task CreateNew(T item)
		{
			data.Add(item.Id, item);
		}

		public async Task Replace(Guid id, T item)
		{
			if (data.ContainsKey(id))
			{
				data[id] = item;
			}
			else
			{
				throw new KeyNotFoundException("There is no item with that id");
			}
		}

		public async Task<T> GetById(Guid id)
		{
			return data[id];
		}

		public async Task<List<T>> GetAll()
		{
			return data.Values.ToList();
		}

		public IQueryable<T> GetQueryable()
		{
			return data.Values.AsQueryable<T>();
		}

		public Task<List<T>> ToListAsync(IQueryable<T> query)
		{
			return Task.FromResult(query.ToList());
		}
	}
}