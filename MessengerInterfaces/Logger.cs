namespace MessengerInterfaces;

public class Logger
{
	public List<(DateTime time, string message, string color)> Logs { get; } = [];

	public void Log(string message, string sender = "Default", string colour = "#fff")
	{
		Logs.Add((DateTime.UtcNow, "[" + sender + "]: " + message, colour));
	}
}