using MessengerInterfaces.Pay;
using MessengerInterfaces.Security;
using MessengerInterfaces.Security.Pay;

namespace CactusPay;

public class Payment(PaymentService paymentService)
{
	public string GeneratePaymentLink(Guid merchantId,
		Guid transactionId,
		DateTime issuingDate,
		TimeSpan expiryTime,
		float amount,
		float currentBalance,
		string description,
		Guid[] recipients,
		int uses = 1)
	{
		PaymentToken token = new(merchantId, transactionId, issuingDate, expiryTime, amount, description, recipients);

		Transaction transaction = new(token, currentBalance - amount);
		paymentService.RegisterTransaction(transaction, uses);

		return $"pay/transaction?token={TokenVerification.GetTokenString(token)}";
	}
}