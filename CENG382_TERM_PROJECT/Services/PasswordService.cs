using Microsoft.Extensions.Configuration;
using BCrypt.Net;

namespace CENG382_TERM_PROJECT.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly IConfiguration _configuration;

        public PasswordService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string HashPassword(string password)
        {
            var pepper = _configuration["Security:Pepper"];
            var passwordWithPepper = password + pepper;
            return BCrypt.Net.BCrypt.HashPassword(passwordWithPepper);
        }
    }
}
