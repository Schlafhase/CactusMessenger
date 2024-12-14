namespace MessengerInterfaces;

public class Logger
{
	public List<(DateTime time, string message)> Logs { get; private set; } = [];
	
	public void Log(string message)
	{
		Logs.Add((DateTime.UtcNow, message));
	}
}