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
            TokenVerification.Initialize();
            AuthorizationToken token = new AuthorizationToken(Guid.NewGuid(), DateTime.UtcNow);
            string tokenString = TokenVerification.GetTokenString(token);
            bool valid = TokenVerification.ValidateToken(tokenString);
            Console.WriteLine(valid);
        }
    }
}