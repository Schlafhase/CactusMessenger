using System.Net.Mail;

namespace EmailService
{
	public class EmailService
	{
		SmtpClient smtpClient;

		public EmailService(string password)
		{
			smtpClient = new SmtpClient("smtp.office365.com", 587);
			smtpClient.EnableSsl = true;
			//smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
			// set smtp-client with basicAuthentication
			smtpClient.UseDefaultCredentials = false;
			System.Net.NetworkCredential basicAuthenticationInfo = new("linus.schneeberg@schlafhase.uk", password);
			smtpClient.Credentials = basicAuthenticationInfo;
		}

		public void Send(string receiver, string subject, string body)
		{
			MailAddress from = new MailAddress("linus.schneeberg@schlafhase.uk", "Cactus Messenger");
			MailAddress to = new MailAddress(receiver);
			using (MailMessage mail = new(from, to)
			{
				// set subject and encoding
				Subject = subject,
				SubjectEncoding = System.Text.Encoding.UTF8,

				// set body-message and encoding
				Body = body,
				BodyEncoding = System.Text.Encoding.UTF8,
				// text or html
				IsBodyHtml = true
			})
			{
				smtpClient.Send(mail);
			}
		}
	}
}
