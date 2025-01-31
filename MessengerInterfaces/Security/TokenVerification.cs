using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace MessengerInterfaces.Security;

public static class TokenVerification
{
	private static byte[] privateKey;
	public static byte[] PublicKey;

	public static void Initialize()
	{
		using (StreamReader sr = new("./privateKey.privkey"))
		{
			privateKey = Convert.FromBase64String(sr.ReadLine()!);
		}


		using (StreamReader sr2 = new("./publicKey.pubkey"))
		{
			PublicKey = Convert.FromBase64String(sr2.ReadLine()!);
		}
	}

	public static (byte[], byte[]) CreateKeyPair()
	{
		RSA rsa = RSA.Create();
		byte[] publicKey = rsa.ExportRSAPublicKey();
		byte[] privateKey = rsa.ExportRSAPrivateKey();
		return (publicKey, privateKey);
	}

	public static string GetTokenString<T>(T token) where T : IToken
	{
		string tokenAsString = JsonConvert.SerializeObject(token);
		byte[] signature = signData(tokenAsString);
		SignedToken<T> signedToken = new(token, signature);
		string signedTokenAsString = JsonConvert.SerializeObject(signedToken);
		string signedTokenBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(signedTokenAsString));
		return signedTokenBase64;
	}

	public static bool ValidateToken<T>(string signedTokenBase64) where T : IToken
	{
		string signedTokenAsString = Encoding.UTF8.GetString(Convert.FromBase64String(signedTokenBase64));
		SignedToken<T> signedToken = JsonConvert.DeserializeObject<SignedToken<T>>(signedTokenAsString);
		T token = signedToken.Token;
		string tokenAsString = JsonConvert.SerializeObject(token);
		return verifyData(tokenAsString, signedToken.Signature, PublicKey);
	}
	
	/// <summary>
	/// Retrieves a <see cref="SignedToken{T}"/> from a base64 encoded signed token.
	/// </summary>
	/// <param name="base64EncodedToken">The encoded token</param>
	/// <param name="validate">Whether the signature should be validated or not</param>
	/// <typeparam name="T">The type of the encoded <see cref="IToken"/></typeparam>
	/// <returns>The decoded token as <see cref="SignedToken{T}"/></returns>
	/// <exception cref="InvalidCredentialException">Throws when the signature is invalid</exception>
	/// <exception cref="ArgumentNullException">Throws when the token is null</exception>
	/// <exception cref="FormatException">Throws when the token isn't in a valid base64 format</exception>
	public static SignedToken<T> GetTokenFromString<T>(string base64EncodedToken, bool validate = true) where T : IToken
	{
		ArgumentNullException.ThrowIfNull(base64EncodedToken, nameof(base64EncodedToken));
		
		if (validate && !ValidateToken<T>(base64EncodedToken))
		{
			throw new InvalidCredentialException("The signature of the token is invalid.");
		}
		
		SignedToken<T> signedToken =
			JsonConvert.DeserializeObject<SignedToken<T>>(
				Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedToken)));
		return signedToken;
	}

	private static byte[] signData(string data)
	{
		using RSACryptoServiceProvider rsa = new();

		rsa.ImportRSAPrivateKey(privateKey, out _);
		RSAParameters rsaParams = rsa.ExportParameters(true);
		byte[] bytes = rsa.SignData(Encoding.UTF8.GetBytes(data), SHA256.Create());
		return bytes;
	}

	private static bool verifyData(string data, byte[] signature, byte[] publicKey)
	{
		using RSACryptoServiceProvider rsa = new();

		rsa.ImportRSAPublicKey(publicKey, out _);
		RSAParameters rsaParams = rsa.ExportParameters(false);
		return rsa.VerifyData(Encoding.UTF8.GetBytes(data), SHA256.Create(), signature);
	}
}