using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CENG382_TERM_PROJECT.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using CENG382_TERM_PROJECT.Utils;
using Microsoft.Extensions.Caching.Memory;
using CENG382_TERM_PROJECT.Services;

namespace CENG382_TERM_PROJECT.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IDataProtector _protector;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private readonly ISystemLogService _systemLogService;

        public LoginModel(AppDbContext context, IDataProtectionProvider provider, IConfiguration configuration, IMemoryCache cache, ISystemLogService systemLogService)
        {
            _context = context;
            _protector = provider.CreateProtector("CENG382_TERM_PROJECT_CookieProtector");
            _configuration = configuration;
            _cache = cache;
            _systemLogService = systemLogService;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            var username = HttpContext.Session.GetString("username");
            var role = HttpContext.Session.GetString("role");

            if (!string.IsNullOrEmpty(username))
            {
                if (role == "Instructor")
                    return RedirectToPage("/Instructor/Index");
                else if (role == "Admin")
                    return RedirectToPage("/Admin/Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                ErrorMessage = "Email ve şifre gereklidir.";
                return Page();
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            var failedAttempt = await _context.FailedLoginAttempts
                .FirstOrDefaultAsync(f => f.IPAddress == ipAddress && f.Email == Email);

            if (failedAttempt != null && failedAttempt.BannedUntil != null && failedAttempt.BannedUntil > DateTime.UtcNow)
            {
                ErrorMessage = $"Çok fazla hatalı giriş denemesi. {failedAttempt.BannedUntil.Value.ToLocalTime()} saatinden sonra tekrar deneyin.";
                return Page();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email);
            var pepper = _configuration["Security:Pepper"];
            var passwordWithPepper = Password + pepper;

            if (user == null || !BCrypt.Net.BCrypt.Verify(passwordWithPepper, user.PasswordHash))
            {
                ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                var role = user?.Role ?? "Instructor";

                failedAttempt = await _context.FailedLoginAttempts
                    .FirstOrDefaultAsync(f => f.IPAddress == ipAddress && f.Email == Email);

                if (failedAttempt == null)
                {
                    failedAttempt = new FailedLoginAttempt
                    {
                        IPAddress = ipAddress,
                        Email = Email,
                        Role = role,
                        AttemptCount = 1,
                        LastAttemptTime = DateTime.UtcNow
                    };
                    _context.FailedLoginAttempts.Add(failedAttempt);
                }
                else
                {
                    failedAttempt.AttemptCount++;
                    failedAttempt.LastAttemptTime = DateTime.UtcNow;
                }

                if ((role == "Instructor" && failedAttempt.AttemptCount >= 5) || (role == "Admin" && failedAttempt.AttemptCount >= 3))
                {
                    failedAttempt.BannedUntil = role == "Admin"
                        ? DateTime.UtcNow.AddMinutes(30)
                        : DateTime.UtcNow.AddMinutes(15);
                }

                await _context.SaveChangesAsync();

                await _systemLogService.LogAsync(null, "LoginFailed",
                    $"Failed login attempt for email: {Email}, IP: {ipAddress}.", false);

                ErrorMessage = "Geçersiz email veya şifre.";
                await Task.Delay(Random.Shared.Next(500, 1500));
                return Page();
            }

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("UserId", user.Id.ToString())
                };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignOutAsync();
            HttpContext.Session.Clear();

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddMinutes(30),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax
            };

            var token = SecurityHelper.GenerateSecureToken(user.Email);
            var sessionId = Guid.NewGuid().ToString();

            HttpContext.Session.SetString("username", user.Email);
            HttpContext.Session.SetString("token", token);
            HttpContext.Session.SetString("session_id", sessionId);
            HttpContext.Session.SetString("role", user.Role);
            _cache.Set(user.Email + "_token", token, TimeSpan.FromMinutes(30));

            Response.Cookies.Append("username", _protector.Protect(user.Email), cookieOptions);
            Response.Cookies.Append("token", _protector.Protect(token), cookieOptions);
            Response.Cookies.Append("session_id", _protector.Protect(sessionId), cookieOptions);

            await _systemLogService.LogAsync(user.Id, "LoginSuccess",
                $"Successful login for email: {user.Email}, IP: {ipAddress}.", true);

            if (user.Role == "Instructor")
            {
                await Task.Delay(Random.Shared.Next(500, 1500));
                return RedirectToPage("/Instructor/Index");
            }
            else if (user.Role == "Admin")
            {
                await Task.Delay(Random.Shared.Next(500, 1500));
                return RedirectToPage("/Admin/Index");
            }
            return RedirectToPage("/Index");
        }
    }
}