using Acornbrot.LocalDB.Interfaces;

namespace MessengerInterfaces.Local;

public interface ILocalObject : IDbObject
{
	public string Type { get; }
}