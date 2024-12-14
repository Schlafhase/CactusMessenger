namespace CactusFrontEnd.Security;

public class SignedToken<T> : ISignedToken where T : IToken
{
	public SignedToken(T Token, byte[] signature)
	{
		this.Token = Token;
		Signature  = signature;
	}

	public byte[] Signature { get; }
	public T      Token     { get; }
}