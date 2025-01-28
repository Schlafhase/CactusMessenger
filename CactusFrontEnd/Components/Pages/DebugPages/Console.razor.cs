using CactusFrontEnd.Security;

namespace CactusFrontEnd.Components.Pages.DebugPages;

public partial class Console : AuthorizedPage
{
	private string _input = "";
	
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			await Initialize(() => navigationManager.NavigateTo("logout?redirectUrl=console"));

			if (user is null)
			{
				return;
			}

			await InvokeAsync(StateHasChanged);
		}
	}
}