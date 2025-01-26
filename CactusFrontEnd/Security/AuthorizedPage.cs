using System.Security.Cryptography;
using CactusFrontEnd.Components;
using CactusFrontEnd.Events;
using Messenger;
using MessengerInterfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace CactusFrontEnd.Security;

public abstract class AuthorizedPage : ComponentBase
{
	protected StreakAlert? _streakAlert;
	protected bool alertShown;
	protected string errorText;
	protected SignedToken<AuthorizationToken> signedToken;
	private string tokenString;
	protected bool updateStreak = true;
	protected bool isMainPage = true;
	protected Account? user;
	[Inject] protected NavigationManager navigationManager { get; set; }
	[Inject] protected EventService eventService { get; set; }
	[Inject] private ProtectedLocalStorage protectedLocalStore { get; set; }
	[Inject] protected IMessengerService messengerService { get; set; }
	[Inject] protected CleanUpService cleanUpService { get; set; }

	protected async Task Initialize(Action action)
	{
		//Action gets called when the user is unauthorized

		if (isMainPage)
		{
			cleanUpService.RunCleanUpIfTimeLimitIsExceeded();
		}

		ProtectedBrowserStorageResult<string> result;

		try
		{
			result = await protectedLocalStore.GetAsync<string>(CactusConstants.AuthTokenKey);
		}
		catch (TaskCanceledException)
		{
			action.Invoke();
			return;
		}
		catch (CryptographicException)
		{
			action.Invoke();
			return;
		}

		tokenString = result.Value;

		if (tokenString is null)
		{
			action.Invoke();
			return;
		}

		try
		{
			signedToken = TokenVerification.GetTokenFromString<AuthorizationToken>(tokenString);
			user = await messengerService.GetAccount(signedToken.Token.UserId);
		}
		catch (Exception e)
		{
			user = null;
			errorText = e.Message;
			alertShown = true;
			action.Invoke();
			return;
		}

		if (user.Locked)
		{
			user = null;

			try
			{
				await protectedLocalStore.DeleteAsync(CactusConstants.AuthTokenKey);
				eventService.TokenHasChanged();
			}
			finally
			{
				navigationManager.NavigateTo("accountLocked");
			}
		}

		if (TokenVerification.AuthorizeUser(tokenString, action))
		{
			if (!updateStreak)
			{
				return;
			}

			int daysSinceLastStreakIncrease = (DateTime.UtcNow.Date - user.LastStreakChange.Date).Days;

			switch (daysSinceLastStreakIncrease)
			{
				case 1:
					await messengerService.UpdateAccountLoginStreak(user.Id, user.LoginStreak + 1);
					user.LoginStreak++;
					await streakIncrease(user.LoginStreak);
					break;
				case > 1:
					await messengerService.UpdateAccountLoginStreak(user.Id, 1);
					
					if (user.LoginStreak > 1)
					{
						await streakLost(user.LoginStreak);
					}

					user.LoginStreak = 1;
					break;
			}

			messengerService.UpdateAccountLastLogin(user.Id, DateTime.UtcNow);
			user.LastLogin = DateTime.UtcNow;
		}
	}

	protected virtual async Task streakIncrease(int newStreak)
	{
		await _streakAlert?.ShowStreakIncreaseAlert(newStreak);
	}

	protected virtual async Task streakLost(int prevStreak)
	{
		await _streakAlert?.ShowStreakLostAlert(prevStreak);
	}
}