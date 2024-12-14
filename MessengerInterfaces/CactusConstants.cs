namespace MessengerInterfaces;

public static class CactusConstants
{
	public static Guid AdminId { get; } = Guid.Empty;
	public static Guid DeletedId { get; } = Guid.Parse("00000000-0000-0000-0000-000000000001");
	public static Guid AdminChannelId { get; } = Guid.Parse("00000000-0000-0000-0000-000000000002");
	public static Guid DeletedChannelId { get; } = Guid.Parse("00000000-0000-0000-0000-000000000003");
	public static Guid EveryoneId { get; } = Guid.Parse("00000000-0000-0000-0000-000000000004");
	public static Guid GlobalChannelId { get; } = Guid.Parse("00000000-0000-0000-0000-000000000005");

	public static Guid PaymentManagerId { get; } = Guid.Parse("00000000-0000-0000-0000-000000000006");

	public static string AuthTokenKey => "CactusMessengerAuthToken";
}