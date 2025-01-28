using MessengerInterfaces;

namespace MessengerInterfaces.Local;

public class LocalAccountRepository(string root)
	: LocalRepositoryBase<Account>(root, "account");