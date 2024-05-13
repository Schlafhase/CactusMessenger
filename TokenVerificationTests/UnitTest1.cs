using CactusFrontEnd.Security;
using FluentAssertions;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;
using System.Text;

namespace TokenVerificationTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GenerateKeyPairTest()
        {
            AuthorizationToken token = new(Guid.NewGuid(), DateTime.UtcNow);
            //string tokenAsString = JsonConvert.SerializeObject(token);
            (byte[], byte[]) keypair = TokenVerification.CreateKeyPair();
            //byte[] signature = TokenVerification.SignData(tokenAsString, keypair.Item2);
            string tokenString = TokenVerification.GetTokenString(token, keypair.Item2);
            Console.WriteLine(tokenString);
            Console.WriteLine(TokenVerification.ValidateToken(tokenString, keypair.Item1));
            Console.WriteLine(Encoding.UTF8.GetString(Convert.FromBase64String(tokenString)));
        }
    }
}