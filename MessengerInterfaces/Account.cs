using MessengerInterfaces;
using MessengerInterfaces.Pay;
using Newtonsoft.Json;

namespace Messenger;

public class Account : ICosmosObject, IInMemoryObject
{
	[System.Text.Json.Serialization.JsonConstructor]
#pragma warning disable CS8618, CS9264
	private Account() { }
#pragma warning restore CS8618, CS9264

	public Account(string userName, string passwordHash) : this(userName, passwordHash, Guid.NewGuid(), null) { }

	public Account(string userName, string passwordHash, string email) : this(
		userName, passwordHash, Guid.NewGuid(), email) { }

	public Account(string userName, string passwordHash, Guid id, string? email)
	{
		CreationDate = DateTime.UtcNow;
		Id = id;
		UserName = userName;
		PasswordHash = passwordHash;
		IsAdmin = false;
		Locked = true;
		Email = email;
	}

	[Newtonsoft.Json.JsonConstructor]
	public Account(List<Transaction>? Transactions, DateTime? LastLogin, DateTime? LastStreakChange)
	{
		this.Transactions = Transactions ?? [];
		this.LastLogin = LastLogin ?? DateTime.MinValue;
		this.LastStreakChange = LastStreakChange ?? DateTime.MinValue;
	}

	public string UserName { get; private set; }
	public string PasswordHash { get; set; }
	public bool IsAdmin { get; set; }
	public bool Locked { get; set; }
	public string? Email { get; set; }
	public DateTime CreationDate { get; set; }
	public float Balance { get; set; }
	public List<Transaction> Transactions { get; set; }
	public DateTime LastLogin { get; set; }
	public int LoginStreak { get; set; }
	public DateTime LastStreakChange { get; set; }

	[JsonProperty("id")] public Guid Id { get; private set; }

	public string Type => "account";
}