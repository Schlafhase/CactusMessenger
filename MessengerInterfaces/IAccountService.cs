namespace MessengerInterfaces;

public interface IAccountService
{
	Task<bool> LoginAccount(Guid id, string password);

	Task<Guid> CreateAccount(string username, string password, string? email = null, bool demo = false);
	Task<Account> GetAccount(Guid Id);
	Task<List<Account>> GetAllAccounts();
	Task<List<Account>> GetExpiredDemoAccounts();
	Task<Account> GetAccountByUsername(string username);
	Task EditAccountAdmin(Guid Id, bool giveAdmin);
	Task EditAccountLock(Guid Id, bool newState);
	Task EditAccountEmail(Guid Id, string email);
	Task ChangePW(Guid Id, string newPW);
	Task ChangePWHash(Guid Id, string newPWHash);
	Task UpdateAccountBalance(Guid Id, float amount);
	Task UpdateAccountLastLogin(Guid Id, DateTime date);
	Task UpdateAccountLoginStreak(Guid Id, int streak);
	Task UpdateTotalMessagesSent(Guid Id, int amount);
	Task UpdateTotalChannelsCreated(Guid Id, int amount);
	Task UpdateProfilePicture(Guid Id, string? picture);
	Task DeleteAccount(Guid Id);
}