using CactusFrontEnd.Security;
using JetBrains.Annotations;
using MessengerInterfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CactusFrontEnd.Components.Pages.DebugPages;

[PublicAPI]
public sealed partial class Log : AuthorizedPage, IDisposable
{
	[Inject] private Logger _logger { get; set; }
	private List<(DateTime time, string message, string color)> _logs = [];
	private bool _disposed;

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
			_logger.OnLogAsync += async () => await refreshLogs();

			// TimerCallback timerCallback = new TimerCallback(async _ => await refreshLogs());
			// _timer = new Timer(timerCallback, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));
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

	public void Dispose()
	{
		_disposed = true;
	}
}