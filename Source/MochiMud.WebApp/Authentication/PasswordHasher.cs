using System.Security.Cryptography;

namespace MochiMud.WebApp.Authentication
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSizeInBytes = 16;
        private const int HashSizeInBytes = 32;
        private const int DefaultIterations = 100_000;

        public PasswordHash Hash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSizeInBytes);
            var hash = DeriveHash(password, salt, DefaultIterations);

            return new PasswordHash(
                Convert.ToBase64String(hash),
                Convert.ToBase64String(salt),
                DefaultIterations);
        }

        public bool Verify(string password, string hash, string salt, int iterations)
        {
            byte[] expectedHash;
            byte[] saltBytes;

            try
            {
                expectedHash = Convert.FromBase64String(hash);
                saltBytes = Convert.FromBase64String(salt);
            }
            catch (FormatException)
            {
                return false;
            }

            var actualHash = DeriveHash(password, saltBytes, iterations);

            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }

        private static byte[] DeriveHash(string password, byte[] salt, int iterations)
        {
            return Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                HashSizeInBytes);
        }
    }
}
