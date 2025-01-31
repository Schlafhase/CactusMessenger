using System.Security.Authentication;
using MessengerInterfaces;
using MessengerInterfaces.Security;
using Microsoft.AspNetCore.Components;

namespace CactusFrontEnd.Components.Pages.MessengerPages;

public partial class ResetPasswordEmailRedirect
{
	[SupplyParameterFromQuery(Name = "token")]
	private string _tokenB64 {get; set;}
	[Inject]
	private IMessengerService _messengerService {get; set;}
	private SignedToken<PasswordResetToken> _token;
	private string _text;
	
	protected override async Task OnInitializedAsync()
	{
		if (string.IsNullOrEmpty(_tokenB64))
		{
			_text = "The provided token is invalid.";
			await InvokeAsync(StateHasChanged);
			return;
		}

		try
		{
			_token = TokenVerification.GetTokenFromString<PasswordResetToken>(_tokenB64);
		}
		catch (InvalidCredentialException e)
		{
			_text = e.Message;
			await InvokeAsync(StateHasChanged);
			return;
		}
		catch
		{
			_text = "The provided token is invalid.";
			await InvokeAsync(StateHasChanged);
			return;
		}
		
		
		if (DateTime.UtcNow - _token.Token.IssuingDate > CactusConstants.PasswordResetTokenLifetime)
		{
			_text = "The provided token has expired.";
			await InvokeAsync(StateHasChanged);
			return;
		}

		try
		{
			await _messengerService.ChangePWHash(_token.Token.UserId, _token.Token.NewPasswordHash);
			_text = "Password changed successfully.";
		}
		catch
		{
			_text = "An error occurred while changing the password.";
		}
		
		await InvokeAsync(StateHasChanged);
	}
}