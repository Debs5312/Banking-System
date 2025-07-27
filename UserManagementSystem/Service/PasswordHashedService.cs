using System.Security.Cryptography;

namespace UserManagementSystem.Service
{
    public static class PasswordHashedService
    {
        // As of 2024, OWASP recommends higher iteration counts for PBKDF2.
        // For example, 600,000 for PBKDF2-SHA256.
        // Adjust this value based on your hardware and security requirements.
        private const int Iterations = 100000;
        private const int SaltSize = 16; // 128 bits
        private const int KeySize = 32; // 256 bits
        private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

        private const char SegmentDelimiter = ':';

        public static string Hash(string input)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                input,
                salt,
                Iterations,
                Algorithm,
                KeySize
            );
            return string.Join(
                SegmentDelimiter,
                Convert.ToHexString(hash),
                Convert.ToHexString(salt),
                Iterations,
                Algorithm
            );
        }

        public static bool Verify(string input, string hashString)
        {
            try
            {
                string[] segments = hashString.Split(SegmentDelimiter);
                if (segments.Length != 4)
                {
                    return false; // Invalid hash format
                }
                byte[] hash = Convert.FromHexString(segments[0]);
                byte[] salt = Convert.FromHexString(segments[1]);
                int iterations = int.Parse(segments[2]);
                var algorithm = new HashAlgorithmName(segments[3]);
                byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(
                    input,
                    salt,
                    iterations,
                    algorithm,
                    hash.Length
                );
                return CryptographicOperations.FixedTimeEquals(inputHash, hash);
            }
            catch (Exception)
            {
                // Any exception during verification (e.g., FormatException)
                // indicates a malformed hash. For security, we return false.
                return false;
            }
        }
    }
}