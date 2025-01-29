namespace Email;

public class LocalEmailService : IEmailService
{
	public void Send(string receiver, string subject, string body)
	{
		Console.WriteLine($"Sending email to {receiver} with subject {subject} and body {body}");
	}
	public void Send(string[] receivers, string subject, string body) { }
}