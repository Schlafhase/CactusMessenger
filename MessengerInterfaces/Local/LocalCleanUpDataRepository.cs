namespace MessengerInterfaces.Local;

public class LocalCleanUpDataRepository(string root)
	: LocalRepositoryBase<CleanUpData>(root, "cleanUpData");