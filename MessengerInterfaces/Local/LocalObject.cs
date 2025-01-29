namespace MessengerInterfaces.Local;

public class LocalObject : ILocalObject
{
	public Guid Id { get; set; }
	public string Type { get; set; }
}