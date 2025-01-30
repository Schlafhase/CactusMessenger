namespace MessengerInterfaces;

public static class CactusConstants
{
	public static string LocalDbRoot { get; } = "db";
	
	public static Guid AdminId => Guid.Empty;
	public static Guid DeletedId { get; } = Guid.Parse("00000000-0000-0000-0000-000000000001");
	public static Guid AdminChannelId { get; } = Guid.Parse("00000000-0000-0000-0000-000000000002");
	public static Guid DeletedChannelId { get; } = Guid.Parse("00000000-0000-0000-0000-000000000003");
	public static Guid EveryoneId { get; } = Guid.Parse("00000000-0000-0000-0000-000000000004");
	public static Guid GlobalChannelId { get; } = Guid.Parse("00000000-0000-0000-0000-000000000005");

	public static Guid PaymentManagerId { get; } = Guid.Parse("00000000-0000-0000-0000-000000000006");
	public static Guid CleanUpDataId { get; } = Guid.Parse("00000000-0000-0000-0000-000000000007");

	public static string AuthTokenKey => "CactusMessengerAuthToken";
	
	public static (int, int) StandardImageResolution { get; } = (300, 300);
	
	public static TimeSpan DemoAccountLifetime { get; } = TimeSpan.FromMinutes(15);
	public static int DemoAccountMaxMessageCount => 15;
	public static (int, int) DemoAccountImageResolution { get; } = (100, 100);
	public static TimeSpan CleanUpFrequency { get; } = TimeSpan.FromMinutes(10);
	
	public static TimeSpan NewHeaderTimespan { get; } = TimeSpan.FromMinutes(2);
}