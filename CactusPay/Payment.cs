using CactusFrontEnd.Security;
using CactusFrontEnd.Security.Pay;

namespace CactusPay;

public static class Payment
{
	public static string GeneratePaymentLink(Guid merchantId, Guid transactionId, DateTime issuingDate, TimeSpan expiryTime, float amount)
	{
		PaymentToken token = new(merchantId, transactionId, issuingDate, expiryTime, amount);
		return $"pay/transaction?token={TokenVerification.GetTokenString(token)}";
	}
}