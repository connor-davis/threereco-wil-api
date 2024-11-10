using System.Security.Cryptography;
using System.Text;

namespace three_api.Lib.Utilities
{
    public class Password
    {
        public static string HashPassword(string password)
        {
            // Generate a salt
            byte[] salt = new byte[256]; // 16 bytes of salt

            RandomNumberGenerator.Fill(salt);

            // Hash the password with the salt using SHA256

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];

            // Combine password and salt
            Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
            Buffer.BlockCopy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

            // Compute hash
            byte[] hash = SHA256.HashData(saltedPassword);

            // Convert the hash and salt to a base64 string for storage
            string hashString = Convert.ToBase64String(hash);
            string saltString = Convert.ToBase64String(salt);

            // Return the salt and hash (separated by a delimiter, e.g., '$')
            return $"{saltString}${hashString}";
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            // Split the stored hash into salt and hash parts
            var parts = storedHash.Split('$');
            if (parts.Length != 2)
            {
                return false;
            }

            string storedSalt = parts[0];
            string storedHashValue = parts[1];

            // Convert salt and hash back from base64
            byte[] salt = Convert.FromBase64String(storedSalt);
            byte[] expectedHash = Convert.FromBase64String(storedHashValue);

            // Hash the provided password with the stored salt

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];

            Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
            Buffer.BlockCopy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

            byte[] hash = SHA256.HashData(saltedPassword);

            // Compare the computed hash with the stored hash
            for (int i = 0; i < expectedHash.Length; i++)
            {
                if (hash[i] != expectedHash[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
