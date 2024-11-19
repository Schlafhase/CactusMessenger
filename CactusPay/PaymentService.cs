using Messenger;
using Microsoft.Azure.Cosmos;

namespace CactusPay;

public class PaymentService(IAccountService accountService)
{
	public async Task Pay(Guid merchantId, Guid clientId, float amount)
	{
		Account merchant = await accountService.GetAccount(merchantId);
		Account client   = await accountService.GetAccount(clientId);

		if (client.Balance < amount)
		{
			throw new ArgumentException("Insufficient funds");
		}

		await accountService.UpdateAccountBalance(client.Id,   client.Balance   - amount);
		await accountService.UpdateAccountBalance(merchant.Id, merchant.Balance + amount);
	}
}