// See https://aka.ms/new-console-template for more information

using System.Buffers.Text;
using System.Text;
using CactusFrontEnd.Security;

(byte[], byte[]) keyPair = TokenVerification.CreateKeyPair();

Console.WriteLine(Convert.ToBase64String(keyPair.Item1));
Console.WriteLine();
Console.WriteLine(Convert.ToBase64String(keyPair.Item2));