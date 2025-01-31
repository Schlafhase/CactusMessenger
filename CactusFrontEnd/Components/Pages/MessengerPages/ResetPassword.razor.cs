using CactusFrontEnd.Security;
using Email;
using MessengerInterfaces;
using Microsoft.AspNetCore.Components;

namespace CactusFrontEnd.Components.Pages.MessengerPages;

public partial class ResetPassword
{
	[Inject]
	private IEmailService _emailService { get; set; }
	[Inject]
	private IMessengerService _messengerService { get; set; }
	private string _username;
	private string _usernameError { get; set; }
	private string _newPassword = "";
	private string _errorString = "";
	private bool _noEmail;
	private bool _disabled;
	private bool _finished;
	private bool _showPassword;
	
	private async Task reset()
	{
		if (_newPassword.Length < 4)
		{
			_errorString = "Please provide a password with at least 4 characters.";
			await InvokeAsync(StateHasChanged);
			return;
		}
		_errorString = "";

		Account user;

		try
		{
			user = await _messengerService.GetAccountByUsername(_username);
		}
		catch
		{
			_usernameError = "User not found";
			await InvokeAsync(StateHasChanged);
			return;
		}

		if (user.Email is string email)
		{

			_disabled = true;
			_emailService.Send(user.Email!, "Reset Password",
							   EmailService.GeneratePasswordResetEmail(_newPassword, user.Id));
		}
		else
		{
			_noEmail = true;
		}

		_finished = true;
		await InvokeAsync(StateHasChanged);
	}
}