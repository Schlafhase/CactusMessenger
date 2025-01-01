using CactusFrontEnd.Security;
using CactusFrontEnd.Security.Pay;
using CactusPay;
using Microsoft.AspNetCore.Components;

namespace CactusFrontEnd.Components.Pages.PayPages;

public partial class Transaction : AuthorizedPage
{
	private string infoText = "Loading...";
	private bool lockButtons = false;

	private SignedToken<PaymentToken> paymentToken;

	[SupplyParameterFromQuery(Name = "token")]
	private string token { get; set; } = "";

	[Inject] private PaymentService paymentService { get; set; }

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			await Initialize(() => navigationManager.NavigateTo($"logout?redirectUrl=transaction?token={token}"));

			try
			{
				paymentToken = TokenVerification.GetTokenFromString<PaymentToken>(token);
			}
			catch (Exception e)
			{
				infoText = "Invalid token: " + e.Message;
				await InvokeAsync(StateHasChanged);
				return;
			}

			infoText =
				$"Received payment by {paymentToken.Token.MerchantId} for {paymentToken.Token.Amount} with description {paymentToken.Token.Description}";
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task pay()
	{
		try
		{
			// TODO: fix index out of range exception
			await paymentService.Pay(paymentToken);
		}
		catch (Exception e)
		{
			infoText = "Error: " + e.Message;
			await InvokeAsync(StateHasChanged);
		}
	}
}