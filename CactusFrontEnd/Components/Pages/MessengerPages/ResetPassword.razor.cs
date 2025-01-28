using CactusFrontEnd.Security;
using Email;
using Microsoft.AspNetCore.Components;

namespace CactusFrontEnd.Components.Pages.MessengerPages;

// TODO: Implement resetPasswordEmailRedirect page

public partial class ResetPassword : AuthorizedPage
{
	[Inject]
	IEmailService emailService { get; set; }
	private string _newPassword = "";
	private string _errorString = "";
	private bool _disabled = false;
	private bool _finished = false;
	private bool _showPassword = false;
	
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			// ReSharper disable once StringLiteralTypo
			await Initialize(() => navigationManager.NavigateTo("/logout?redirectUrl=messenger/resetpassword"));
			
			if (user is null)
			{
				return;
			}

			if (string.IsNullOrWhiteSpace(user.Email))
			{
				_disabled = true;
				_errorString = "You need to have an email address to reset your password";
				await InvokeAsync(StateHasChanged);
			}
		}
	}
	
	private async Task reset()
	{
		if (_newPassword.Length < 4)
		{
			_errorString = "Please provide a password with at least 4 characters.";
			await InvokeAsync(StateHasChanged);
			return;
		}
		
		_disabled = true;
		emailService.Send(user.Email!, "Reset Password", EmailService.GeneratePasswordResetEmail(_newPassword, user.Id));
		_finished = true;
		await InvokeAsync(StateHasChanged);
	}
}