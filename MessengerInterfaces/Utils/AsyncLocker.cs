namespace CactusFrontEnd.Utils;

public class AsyncLocker
{
	private readonly SemaphoreSlim semaphore;

	public AsyncLocker()
	{
		semaphore = new SemaphoreSlim(1);
	}

	public async Task<IDisposable> Enter()
	{
		await semaphore.WaitAsync();
		return new Releaser(semaphore);
	}

	private class Releaser : IDisposable
	{
		private readonly SemaphoreSlim semaphore;

		public Releaser(SemaphoreSlim sem)
		{
			semaphore = sem;
		}

		public void Dispose()
		{
			semaphore.Release();
		}
	}
}