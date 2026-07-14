using System;
using System.Security.Cryptography;

namespace HRSystem.Infrastructure.Security
{
    public class PasswordService
    {
        public string Hash(string password)
        {
            var salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
            {
                var hash = pbkdf2.GetBytes(32);
                return string.Format("{0}.{1}", Convert.ToBase64String(salt), Convert.ToBase64String(hash));
            }
        }

        public bool Verify(string password, string savedHash)
        {
            if (string.IsNullOrWhiteSpace(savedHash))
            {
                return false;
            }

            var parts = savedHash.Split('.');
            if (parts.Length != 2)
            {
                return false;
            }

            var salt = Convert.FromBase64String(parts[0]);
            var expectedHash = Convert.FromBase64String(parts[1]);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
            {
                var actualHash = pbkdf2.GetBytes(32);
                return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
            }
        }
    }
}
