using MessengerInterfaces;

namespace MessengerInterfaces.Local;

public class LocalChannelRepository(string root)
	: LocalRepositoryBase<Channel>(root, "channel");