﻿using System.Text;
using MessengerInterfaces;
using MessengerInterfaces.Security;
using MessengerInterfaces.Security.Pay;
using Newtonsoft.Json;
using Transaction = MessengerInterfaces.Pay.Transaction;

namespace CactusPay;

public class PaymentService(
	IRepository<PaymentManager> paymentRepo,
	IMessengerService messengerService)
{
	public async Task Pay(SignedToken<PaymentToken> signedToken)
	{
		if (!TokenVerification.ValidateToken<PaymentToken>(
			Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(signedToken)))))
		{
			throw new ArgumentException("Invalid token");
		}

		PaymentToken token = signedToken.Token;

		Guid merchantId = token.MerchantId;
		Guid clientId = token.Recipients[0];
		Guid transactionId = token.TransactionId;
		float amount = token.Amount;

		Account merchant = await messengerService.GetAccount(merchantId);
		Account client = await messengerService.GetAccount(clientId);

		if (!token.Recipients.Contains(clientId) && token.Recipients.Length != 0)
		{
			throw new ArgumentException("User is not a recipient of this transaction.");
		}

		if (client.Balance + 1000 < amount)
		{
			throw new ArgumentException("Insufficient funds");
		}

		PaymentManager manager = await getManager() ?? throw new ArgumentException("Payment manager not found.");

		if (!manager.OpenTransactions.TryGetValue(transactionId, out int value))
		{
			throw new ArgumentException("Transaction expired.");
		}

		Action<PaymentManager> update;

		if (value - 1 <= 0)
		{
			update = m => m.OpenTransactions.Remove(transactionId);
		}
		else
		{
			update = m => m.OpenTransactions[transactionId]--;
		}


		await paymentRepo.UpdateItemVoid(CactusConstants.PaymentManagerId, update);

		await messengerService.UpdateAccountBalance(client.Id, client.Balance - amount);
		await messengerService.UpdateAccountBalance(merchant.Id, merchant.Balance + amount);
	}

	public async Task RegisterTransaction(Transaction transaction, int uses)
	{
		await paymentRepo.UpdateItemVoid(CactusConstants.PaymentManagerId,
								   manager => manager.OpenTransactions.Add(transaction.Token.TransactionId, uses));
	}

	private async Task<PaymentManager?> getManager() => await paymentRepo.GetById(CactusConstants.PaymentManagerId);
}