namespace Email;

public interface IEmailService
{
	void Send(string receiver, string subject, string body);
	void Send(string[] receivers, string subject, string body);
}