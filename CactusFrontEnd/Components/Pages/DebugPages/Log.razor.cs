using System.Net;
using CactusFrontEnd.Security;
using JetBrains.Annotations;
using MessengerInterfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CactusFrontEnd.Components.Pages.DebugPages;

[PublicAPI]
public partial class Log : AuthorizedPage
{
	[Inject] private Logger _logger { get; set; }
	private List<(DateTime time, string message, string color)> _logs = [];
	private DateTime _lastRefresh = DateTime.MinValue;
	private Thread _refreshThread;

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			await Initialize(() => { navigationManager.NavigateTo("/logout?redirectUrl=debug/log"); });

			if (user is null)
			{
				return;
			}

			if (!user.IsAdmin)
			{
				navigationManager.NavigateTo("/Error/Unauthorized");
				return;
			}
			
			await refreshLogs(true);
			_lastRefresh = DateTime.UtcNow;
		}
		
		
		if (DateTime.UtcNow - _lastRefresh > TimeSpan.FromMilliseconds(20))
		{
			await refreshLogs();
			_lastRefresh = DateTime.UtcNow;
		}
	}
	
	private async Task clearLogs()
	{
		_logger.Logs.Clear();
		_logs.Clear();
		await InvokeAsync(StateHasChanged);
	}
	
	private async Task refreshLogs(bool firstRender = false)
	{
		_logs = _logger.Logs.ToList();
		await InvokeAsync(StateHasChanged);

		if (firstRender)
		{
			await _jsRuntime.InvokeVoidAsync("scrollToBottom");
		}
	}
}