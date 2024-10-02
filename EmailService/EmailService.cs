using System.Net.Mail;

namespace EmailService
{
	public class EmailService
	{
        public EmailService()
        {
			SmtpClient mySmtpClient = new SmtpClient("smtpout.secureserver.net");

			// set smtp-client with basicAuthentication
			mySmtpClient.UseDefaultCredentials = false;
			System.Net.NetworkCredential basicAuthenticationInfo = new
			   System.Net.NetworkCredential("", "password");
			mySmtpClient.Credentials = basicAuthenticationInfo;
		}
    }
}
