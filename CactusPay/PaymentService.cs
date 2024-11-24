using System.Text;
using CactusFrontEnd.Security;
using CactusFrontEnd.Security.Pay;
using CactusFrontEnd.Utils;
using Messenger;
using MessengerInterfaces;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Transaction = MessengerInterfaces.Pay.Transaction;

namespace CactusPay;

public class PaymentService(IMessengerService accountService, CosmosClient client, AsyncLocker asyncLocker)
{
	private readonly Container container = client.GetContainer("cactus-messenger", "cactus-messenger");

	public async Task Pay(SignedToken<PaymentToken> signedToken)
	{
		PaymentToken token;

		if (!TokenVerification.ValidateToken<PaymentToken>(
			    Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(signedToken)))))
		{
			throw new ArgumentException("Invalid token");
		}
		else
		{
			token = signedToken.Token;
		}

		Guid  merchantId    = token.MerchantId;
		Guid  clientId      = token.Recipients[0];
		Guid  transactionId = token.TransactionId;
		float amount        = token.Amount;

		Account merchant = await accountService.GetAccount(merchantId);
		Account client   = await accountService.GetAccount(clientId);

		if (!token.Recipients.Contains(clientId) && token.Recipients.Length != 0)
		{
			throw new ArgumentException("User is not a recipient of this transaction.");
		}

		if (client.Balance + 1000 < amount)
		{
			throw new ArgumentException("Insufficient funds");
		}

		using IDisposable _ = await asyncLocker.Enter();

		PaymentManager manager = await getManager();

		if (!manager.OpenTransactions.TryGetValue(transactionId, out int value))
		{
			throw new ArgumentException("Transaction expired.");
		}

		if (value - 1 <= 0)
		{
			manager.OpenTransactions.Remove(transactionId);
		}
		else
		{
			manager.OpenTransactions[transactionId]--;
		}


		await container.ReplaceItemAsync(manager, CactusConstants.PaymentManagerId.ToString(),
		                                 new PartitionKey(CactusConstants.PaymentManagerId.ToString()));

		await accountService.UpdateAccountBalance(client.Id,   client.Balance   - amount);
		await accountService.UpdateAccountBalance(merchant.Id, merchant.Balance + amount);
	}

	public async Task RegisterTransaction(Transaction transaction, int uses)
	{
		using IDisposable _       = await asyncLocker.Enter();
		PaymentManager    manager = await getManager();
		manager.OpenTransactions.Add(transaction.Token.TransactionId, uses);
		await container.ReplaceItemAsync(manager, CactusConstants.PaymentManagerId.ToString(),
		                                 new PartitionKey(CactusConstants.PaymentManagerId.ToString()));
	}

	private async Task<PaymentManager> getManager()
	{
		IQueryable<PaymentManager> q = container.GetItemLinqQueryable<PaymentManager>()
		                                        .Where(item => item.Id == CactusConstants.PaymentManagerId);
		IAsyncEnumerable<PaymentManager> response = Utils.ExecuteQuery(q);
		List<PaymentManager>             result   = await Utils.ToListAsync(response);

		try
		{
			return result[0];
		}
		catch (IndexOutOfRangeException e)
		{
			throw new Exception("PaymentManager not found", e);
		}
	}
}