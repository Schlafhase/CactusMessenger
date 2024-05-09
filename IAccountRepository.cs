namespace Messenger
{
    public interface IAccountRepository
    {
        Task<Guid> CreateAccount(string username);
        Task<Account> GetAccount(Guid Id);
        Task EditAccountAdmin(Guid Id, bool giveAdmin, Guid userId, IAccountRepository accountRepo);

	}
}
