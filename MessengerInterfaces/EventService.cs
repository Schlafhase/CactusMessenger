namespace MessengerInterfaces;

public class EventService
{
	public event Action? OnTokenChange;

	public void TokenHasChanged()
	{
		OnTokenChange?.Invoke();
	}

	public event Action? OnChannelListChange;

	public void ChannelsHaveChanged()
	{
		OnChannelListChange?.Invoke();
	}

	public event Action? OnChannelChange;

	public void ChannelHasChanged()
	{
		OnChannelChange?.Invoke();
	}
}