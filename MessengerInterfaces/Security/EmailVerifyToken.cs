namespace CactusFrontEnd.Security;

public class EmailVerifyToken(string email, Guid userId, DateTime issuingDate) : IToken
{
	public string   Email       { get; private set; } = email;
	public Guid     UserId      { get; private set; } = userId;
	public DateTime IssuingDate { get; private set; } = issuingDate;
}