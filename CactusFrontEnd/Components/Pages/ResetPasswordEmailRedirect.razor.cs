using CactusFrontEnd.Security;
using MessengerInterfaces;
using MessengerInterfaces.Security;
using Microsoft.AspNetCore.Components;

namespace CactusFrontEnd.Components.Pages;

public partial class ResetPasswordEmailRedirect : ComponentBase
{
	[Parameter]
	[SupplyParameterFromQuery(Name = "token")]
	public string passwordResetToken { get; set; } = "";

	[Inject] 
	private IMessengerService messengerService { get; set; }

	private string header = "Loading...";
	private string errorText = "";
	private bool alertShown = false;

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			try
			{
				if (TokenVerification.ValidateToken<PasswordResetToken>(passwordResetToken))
				{
					SignedToken<PasswordResetToken> token =
						TokenVerification.GetTokenFromString<PasswordResetToken>(passwordResetToken);

					if (DateTime.UtcNow - token.Token.IssuingDate > TimeSpan.FromMinutes(10))
					{
						header = "Failed to reset password.";
						errorText = "The password reset token has expired.";
						alertShown = true;
					}
					else
					{

						await messengerService.ChangePWHash(token.Token.UserId, token.Token.NewPasswordHash);
						header = "Your password has been reset successfully.";
					}

					await InvokeAsync(StateHasChanged);
				}
			}
			catch (Exception ex)
			{
				errorText = ex.Message;
				alertShown = true;
				header = "Failed to reset password";
				await InvokeAsync(StateHasChanged);
			}
		}
	}
}