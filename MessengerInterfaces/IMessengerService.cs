using MessengerInterfaces;

namespace MessengerInterfaces;

public interface IMessengerService : IMessageService, IChannelService, IAccountService
{
	Task InitializeAsync();
	Task<CleanUpData> GetCleanUpData();
	Task SaveCleanUpData(CleanUpData data);
}