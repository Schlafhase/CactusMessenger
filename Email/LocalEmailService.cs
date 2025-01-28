namespace Email;

public class LocalEmailService : IEmailService
{
	public void Send(string receiver, string subject, string body) { }
	public void Send(string[] receivers, string subject, string body) { }
}