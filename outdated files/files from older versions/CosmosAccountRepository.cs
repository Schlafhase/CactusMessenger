using Messenger;
using Microsoft.Azure.Cosmos;
using outdated;

namespace outdated
{
	public class CosmosAccountRepository : CosmosRepositoryBase<Account>, IAccountService
	{
		public CosmosAccountRepository(CosmosClient client) : base(client) { }

		public async Task<Guid> CreateAccount(string username, string passwordHash)
		{
			Account acc = new Account(username, passwordHash);
			await this.Create(acc);
			return acc.Id;
		}

		public async Task EditAccountAdmin(Guid Id, bool giveAdmin, Guid userId)
		{
			Account account = await this.GetById(Id);
			account.IsAdmin = giveAdmin;
			await this.Replace(account, Id.ToString());
		}

		public async Task<Account> GetAccount(Guid Id)
		{
			Account account = await this.GetById(Id);
			return account;
		}
	}
}
