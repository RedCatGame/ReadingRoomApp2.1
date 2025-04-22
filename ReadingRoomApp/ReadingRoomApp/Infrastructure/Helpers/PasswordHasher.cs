using System;
using System.Security.Cryptography;
using System.Text;

namespace ReadingRoomApp.Infrastructure.Helpers
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = $"{password}_{salt}";
                var bytes = Encoding.UTF8.GetBytes(saltedPassword);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public static bool VerifyPassword(string password, string salt, string hash)
        {
            string computedHash = HashPassword(password, salt);
            return computedHash == hash;
        }

        public static string GenerateSalt(string username)
        {
            return $"salt_for_{username}";
        }
    }
}