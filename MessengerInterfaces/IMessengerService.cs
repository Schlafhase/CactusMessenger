using Messenger;

namespace MessengerInterfaces;

public interface IMessengerService : IMessageService, IChannelService, IAccountService
{
	Task InitializeAsync();
}