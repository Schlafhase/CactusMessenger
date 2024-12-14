namespace CactusFrontEnd.Security;

public class AuthorizationToken(Guid userId, DateTime issuingDate) : IToken
{
	public Guid     UserId      { get; private set; } = userId;
	public DateTime IssuingDate { get; private set; } = issuingDate;
}