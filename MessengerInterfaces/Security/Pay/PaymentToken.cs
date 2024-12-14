namespace CactusFrontEnd.Security.Pay;

public class PaymentToken(Guid merchantId, Guid transactionId, DateTime issuingDate, TimeSpan expiryTime, float amount, string description, Guid[] recipients)
	: IToken
{
	public Guid     MerchantId    { get; private set; } = merchantId;
	public Guid     TransactionId { get; private set; } = transactionId;
	public DateTime IssuingDate   { get; private set; } = issuingDate;
	public TimeSpan ExpiryTime    { get; private set; } = expiryTime;
	public float    Amount        { get; private set; } = amount;
	
	public string Description {get; private set;} = description;
	public Guid[] Recipients {get; private set;} = recipients;
}