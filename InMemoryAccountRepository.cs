
using Microsoft.Extensions.Configuration.UserSecrets;

namespace Messenger
{
    public class InMemoryAccountRepository : IAccountRepository
    {
        public readonly Dictionary<Guid, Account> accounts = new();
        private readonly object locker = new object();

        public InMemoryAccountRepository()
        {
            createAdminAccount();
		}

		public async Task<Guid> CreateAccount(string username)
        {
            Guid userId = Guid.NewGuid();
            lock (locker)
            {
                accounts.Add(userId, new Account(username, userId));
            }
            return userId;
        }

        public async Task<Account> GetAccount(Guid Id)
        {
            try
            {
                return accounts[Id];
            }
            catch (KeyNotFoundException)
            {
                throw new Exception($"Unable to find Account with Id {Id}");
            }
        }

  //      public async Task<Account> CreateAdminAccount(string username = "Admin")
		//{
		//	return createAdminAccount(username);
		//}

        public async Task EditAccountAdmin(Guid Id, bool giveAdmin, Guid userId, IAccountRepository accountRepo)
        {
            Account user = await accountRepo.GetAccount(userId);
            if (user.isAdmin)
            {
                accounts[Id].isAdmin = giveAdmin;
            }
            else
            {
                throw new Exception("No permissions (Only admins can change the admin status of other users)");
            }
        }

		private Account createAdminAccount(string username = "Admin")
		{
			lock (locker)
			{
				Guid adminUserId = Guid.Empty;
				Account admin = new Account(username, adminUserId);
				admin.isAdmin = true;
				accounts.Add(adminUserId, admin);
				return admin;
			}
		}
	}
}