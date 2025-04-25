using System.Security.Cryptography;
using System.Text;

namespace CENG382_TERM_PROJECT.Utils
{
    public static class SecurityHelper
    {
        public static string GenerateSecureToken(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input + Guid.NewGuid());
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }
    }
}
