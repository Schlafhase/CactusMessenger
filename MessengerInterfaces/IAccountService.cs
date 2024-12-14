namespace Messenger;

public interface IAccountService
{
	Task<bool> LoginAccount(Guid id, string password);

	Task<Guid>      CreateAccount(string username, string password);
	Task<Account>   GetAccount(Guid      Id);
	Task<Account[]> GetAllAccounts();
	Task<Account>   GetAccountByUsername(string   username);
	Task            EditAccountAdmin(Guid         Id, bool     giveAdmin);
	Task            EditAccountLock(Guid          Id, bool     newState);
	Task            EditAccountEmail(Guid         Id, string   email);
	Task            ChangePW(Guid                 Id, string   newPW);
	Task            UpdateAccountBalance(Guid     Id, float    amount);
	Task            UpdateAccountLastLogin(Guid   Id, DateTime date);
	Task            UpdateAccountLoginStreak(Guid Id, int      streak);
	Task            DeleteAccount(Guid            Id);
}