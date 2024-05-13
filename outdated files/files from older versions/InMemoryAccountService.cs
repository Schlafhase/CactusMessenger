
using Messenger;
using MessengerInterfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Security.Cryptography.X509Certificates;

namespace outdated
{
    public class InMemoryAccountService : IAccountService
    {
        public readonly Dictionary<Guid, Account> accounts = new();
        private readonly object locker = new object();

        public InMemoryAccountService(string deletedUserPasswordHash, string adminPasswordHash)
        {
            createAdminAccount(adminPasswordHash);
            createDeletedUser(deletedUserPasswordHash);
		}
        
		public async Task<Guid> CreateAccount(string username, string passwordHash)
        {
            Guid userId = Guid.NewGuid();
            lock (locker)
            {
                accounts.Add(userId, new Account(username, passwordHash, userId));
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

        public async Task EditAccountAdmin(Guid Id, bool giveAdmin, Guid userId)
        {
            Account user = await this.GetAccount(userId);
            if (user.IsAdmin)
            {
                accounts[Id].IsAdmin = giveAdmin;
            }
            else
            {
                throw new Exception("No permissions (Only admins can change the admin status of other users)");
            }
        }

		private Account createAdminAccount(string passwordHash, string username = "Admin")
		{
			lock (locker)
			{
				Guid adminUserId = CactusConstants.AdminId;
				Account admin = new Account(username, passwordHash, adminUserId);
				admin.IsAdmin = true;
				accounts.Add(adminUserId, admin);
				return admin;
			}
		}

        private Account createDeletedUser(string passwordHash)
        {
			lock (locker)
			{
				Guid deletedUserId = CactusConstants.DeletedId;
				Account deleted = new Account("Deleted User", passwordHash, deletedUserId);
				accounts.Add(deletedUserId, deleted);
				return deleted;
			}
		}
	}
}