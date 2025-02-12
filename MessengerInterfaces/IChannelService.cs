﻿namespace MessengerInterfaces;

public interface IChannelService
{
	Task<ChannelDTO_Output> GetChannel(Guid channelId);
	Task<ChannelDTO_Output[]> GetAllChannels();
	Task<Guid> CreateChannel(HashSet<Guid> userIds, string name, Guid authorId);
	Task<ChannelDTO_Output[]> GetChannelsFromAuthor(Guid authorId);
	Task AddUserToChannel(Guid Id, Guid channelId);
	Task<ChannelDTO_Output[]> GetChannelsWithUser(Guid accountId);
	Task DeleteChannel(Guid Id);
	Task RemoveUserFromChannel(Guid channelId, Guid accountId);
	Task UpdateLastMessageTime(Guid channelId, DateTime time);
	Task UpdateLastRead(Guid channelId, Guid accountId, DateTime time);
}