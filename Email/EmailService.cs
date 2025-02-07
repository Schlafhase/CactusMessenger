﻿using System.Net;
using System.Net.Mail;
using System.Text;
using MessengerInterfaces.Security;
using MessengerInterfaces.Utils;

namespace Email;

public class EmailService : IEmailService
{
	private readonly SmtpClient smtpClient;

	public EmailService(string password)
	{
		smtpClient = new SmtpClient("smtp.office365.com", 587);
		smtpClient.EnableSsl = true;
		//smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
		// set smtp-client with basicAuthentication
		smtpClient.UseDefaultCredentials = false;
		NetworkCredential basicAuthenticationInfo = new("linus.schneeberg@schlafhase.uk", password);
		smtpClient.Credentials = basicAuthenticationInfo;
	}

	public void Send(string receiver, string subject, string body)
	{
		Send([receiver], subject, body);
	}

	public void Send(string[] receivers, string subject, string body)
	{
		MailAddress from = new("linus.schneeberg@schlafhase.uk", "Cactus Messenger");

		using MailMessage mail = new()
		{
			// set subject and encoding
			From = from,
			Subject = subject,
			SubjectEncoding = Encoding.UTF8,

			// set body-message and encoding
			Body = body,
			BodyEncoding = Encoding.UTF8,
			// text or html
			IsBodyHtml = true
		};

		//add recipients
		foreach (string receiver in receivers)
		{
			mail.To.Add(receiver);
		}

		smtpClient.Send(mail);
	}

	private static string generateEmailBase(string header, string body) =>
		$"<div style='padding: 5px'><div id='info' style='border-radius: 20px; max-width: 500px; padding: 20px; color: white; background-color: black;'><h1 style='padding-bottom: 30px;'>{header}</h1>{body}</div></div>";

	public static string GenerateAccountRequestEmail(string username, string description, Guid id, string email, bool demo=false)
	{
		string emailLower = email.ToLower();
		bool addEmail = !string.IsNullOrWhiteSpace(emailLower);

		string body =
			$"<b>Username: </b><span>{username}</span><br/><b>Description: </b><span>{description}</span><br/><div style='padding-top: 50px;'>{(demo ? "" : $"<a style='color: white; text-decoration: none; padding: 5px 10px;background-color: blue; border-radius: 10px;float left;transform: translate(0px, -20px);' href='https://cactusmessenger.azurewebsites.net/reviewAccount?user={id}&action=accept{(addEmail ? "&email=" + emailLower : "")}'>Accept</a><a href='https://cactusmessenger.azurewebsites.net/reviewAccount?user={id}&action=deny{(addEmail ? "&email=" + emailLower : "")}' style='transform: translate(0px, -20px);color: white; text-decoration: none; padding: 5px 10px;float: right; background-color: red; border-radius: 10px;'>Deny</a>")}";
		return generateEmailBase($"New {(demo ? "Demo Account Created" :"Account Creation Request")}", body);
	}

	public static string GenerateVerificationEmail(Guid id, string email)
	{
		EmailVerifyToken token = new(email, id, DateTime.UtcNow);
		string tokenString = TokenVerification.GetTokenString(token);
		string body =
			$"<span>Verify your email address to add it to your account.<br/> <a style='color: skyblue;' href='https://cactusmessenger.azurewebsites.net/emails'>Why should I add my email address to my account?</a></span><div style='padding-top:25px;'><a style='color: white; text-decoration: none; padding: 5px 10px;background-color: blue; border-radius: 10px;' href='https://cactusmessenger.azurewebsites.net/verifyEmail?token={tokenString}'>Verify</a></div>";
		return generateEmailBase("Verify your email address.", body);
	}

	public static string GeneratePasswordResetEmail(string newPassword, Guid id)
	{
		string passwordHash = Utils.GetStringSha256Hash(newPassword + id);
		PasswordResetToken token = new(passwordHash, id, DateTime.UtcNow);
		string tokenString = TokenVerification.GetTokenString(token);
		string body =
			$"<span>A request to reset your Cactus Messenger password has been issued. If this wasn't you just ignore this email. To reset your password, click the button below.<br/> </span><div style='padding-top:25px;'><a style='color: white; text-decoration: none; padding: 5px 10px;background-color: blue; border-radius: 10px;' href='https://cactusmessenger.azurewebsites.net/resetPasswordEmailRedirect?token={tokenString}'>Reset</a></div>";
		return generateEmailBase("Reset Password.", body);
	}

	public static string GenerateAccountReviewEmail(bool accepted)
	{
		string body =
			$"<span>Your account creation request was reviewed and {(accepted ? "accepted" : "denied")} by an administrator.</span><div style='padding-top:25px;'><a style='color: white; text-decoration: none; padding: 5px 10px;background-color: blue; border-radius: 10px;' href='{(accepted ? "https://messenger.schlafhase.uk" : "https://schlafhase.uk#contact")}'>{(accepted ? "Sign in" : "Contact me")}</a><span style='padding-left: 15px;line-height:25px;font-size:12px; font-style:italic;color:gray;'>{(accepted ? "" : "if you think this choice was unfair.")}</span></div>";
		return generateEmailBase($"Your account creation request has been {(accepted ? "accepted" : "denied")}.", body);
	}
}