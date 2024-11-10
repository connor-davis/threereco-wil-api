using System.Security.Cryptography;

namespace three_api.Lib.Helpers
{
    public class AuthenticationSettings
    {
        // The private key will be generated automatically when the class is accessed.
        public static string PrivateKey { get; private set; } = string.Empty;

        // The public key will also be generated automatically from the private key.
        public static string PublicKey { get; private set; } = string.Empty;

        // Method to generate a new RSA key pair (private and public keys)
        public static void GenerateKeys()
        {
            using RSA rsa = RSA.Create(2048);  // Create a new RSA instance with a key size of 2048 bits
            
            PrivateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey()); // Export private key as Base64 string
            PublicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());   // Export public key as Base64 string
        }
    }
}
