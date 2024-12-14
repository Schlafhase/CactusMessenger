namespace MessengerInterfaces;

public class Logger
{
	public List<(DateTime time, string message)> Logs { get; } = [];

	public void Log(string message)
	{
		Logs.Add((DateTime.UtcNow, message));
	}
}