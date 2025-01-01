namespace CactusFrontEnd.Security;

public class PasswordResetToken(string newPasswordHash, Guid userId, DateTime issuingDate) : IToken
{
	public string NewPasswordHash { get; private set; } = newPasswordHash;
	public Guid UserId { get; private set; } = userId;
	public DateTime IssuingDate { get; private set; } = issuingDate;
}