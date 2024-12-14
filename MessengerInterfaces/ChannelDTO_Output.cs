using Messenger;

namespace MessengerInterfaces;

public class ChannelDTO_Output(Channel channel, HashSet<string> userNames)
{
	public Guid                       Id          { get; }      = channel.Id;
	public HashSet<Guid>              Users       { get; }      = channel.Users;
	public HashSet<string>            UserNames   { get; }      = userNames;
	public string                     Name        { get; }      = channel.Name;
	public DateTime                   LastMessage { get; set; } = channel.LastMessage;
	public Dictionary<Guid, DateTime> LastRead    { get; set; } = channel.LastRead;
	

	public static async Task<ChannelDTO_Output> FromChannel(Channel channel, IAccountService accountRepo)
	{
		string[] UserNames = await Task.WhenAll(channel.Users
		                                               .Select(async userId =>
		                                                       {
			                                                       Account user = await accountRepo.GetAccount(userId);
			                                                       return user.UserName;
		                                                       }));
		return new ChannelDTO_Output(channel, UserNames.ToHashSet());
	}
}