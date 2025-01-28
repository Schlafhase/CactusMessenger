using MessengerInterfaces;

namespace MessengerInterfaces.Local;

public class LocalMessageRepository(string root)
	: LocalRepositoryBase<Message>(root, "message");