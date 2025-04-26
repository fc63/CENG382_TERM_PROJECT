using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CENG382_TERM_PROJECT.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Memory;

namespace CENG382_TERM_PROJECT.Pages.Admin
{
	[Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            var sessionUsername = HttpContext.Session.GetString("username");
            var sessionToken = HttpContext.Session.GetString("token");
            var sessionId = HttpContext.Session.GetString("session_id");

            Request.Cookies.TryGetValue("username", out var protectedUsername);
            Request.Cookies.TryGetValue("token", out var protectedToken);
            Request.Cookies.TryGetValue("session_id", out var protectedSessionId);
            string cookieUsername = protectedUsername;
            string cookieToken = protectedToken;
            string cookieSessionId = protectedSessionId;
            string decryptedUsername = null;
            string decryptedToken = null;
            string decryptedSessionId = null;

            try
            {
                if (!string.IsNullOrEmpty(cookieUsername))
                    decryptedUsername = _protector.Unprotect(cookieUsername);
            }
            catch
            {
                decryptedUsername = null;
            }

            try
            {
                if (!string.IsNullOrEmpty(cookieToken))
                    decryptedToken = _protector.Unprotect(cookieToken);
            }
            catch
            {
                decryptedToken = null;
            }

            try
            {
                if (!string.IsNullOrEmpty(cookieSessionId))
                    decryptedSessionId = _protector.Unprotect(cookieSessionId);
            }
            catch
            {
                decryptedSessionId = null;
            }
            if (sessionUsername != decryptedUsername || sessionToken != decryptedToken || sessionId != decryptedSessionId)
            {
                HttpContext.Session.Clear();
                Response.Cookies.Delete("username");
                Response.Cookies.Delete("token");
                Response.Cookies.Delete("session_id");
                return RedirectToPage("/Auth/Login");
            }
            var cacheToken = _cache.Get<string>(sessionUsername + "_token");

            if (cacheToken == null || cacheToken != sessionToken)
            {
                HttpContext.Session.Clear();
                Response.Cookies.Delete("username");
                Response.Cookies.Delete("token");
                Response.Cookies.Delete("session_id");
                return RedirectToPage("/Auth/Login");
            }
            Instructors = _context.Users.Where(u => u.Role == "Instructor").ToList();
            return Page();
        }
        private readonly AppDbContext _context;
        private readonly IDataProtector _protector;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;

        [BindProperty] public string FullName { get; set; }
		[BindProperty] public string Email { get; set; }
		[BindProperty] public string Password { get; set; }
        [BindProperty] public int? EditingId { get; set; }
        public string Message { get; set; }
        public List<User> Instructors { get; set; }
        public IndexModel(AppDbContext context, IDataProtectionProvider provider, IConfiguration configuration, IMemoryCache cache)
        {
            _context = context;
            _protector = provider.CreateProtector("CENG382_TERM_PROJECT_CookieProtector");
            _configuration = configuration;
            _cache = cache;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var sessionUsername = HttpContext.Session.GetString("username");
            var sessionToken = HttpContext.Session.GetString("token");
            var sessionId = HttpContext.Session.GetString("session_id");

            Request.Cookies.TryGetValue("username", out var cookieUsername);
            Request.Cookies.TryGetValue("token", out var cookieToken);
            Request.Cookies.TryGetValue("session_id", out var cookieSessionId);

            string decryptedUsername = null;
            string decryptedToken = null;
            string decryptedSessionId = null;

            try
            {
                if (!string.IsNullOrEmpty(cookieUsername))
                    decryptedUsername = _protector.Unprotect(cookieUsername);
            }
            catch { decryptedUsername = null; }

            try
            {
                if (!string.IsNullOrEmpty(cookieToken))
                    decryptedToken = _protector.Unprotect(cookieToken);
            }
            catch { decryptedToken = null; }

            try
            {
                if (!string.IsNullOrEmpty(cookieSessionId))
                    decryptedSessionId = _protector.Unprotect(cookieSessionId);
            }
            catch { decryptedSessionId = null; }

            if (sessionUsername != decryptedUsername || sessionToken != decryptedToken || sessionId != decryptedSessionId)
            {
                HttpContext.Session.Clear();
                Response.Cookies.Delete("username");
                Response.Cookies.Delete("token");
                Response.Cookies.Delete("session_id");
                return RedirectToPage("/Auth/Login");
            }

            var cacheToken = _cache.Get<string>(sessionUsername + "_token");

            if (cacheToken == null || cacheToken != sessionToken)
            {
                HttpContext.Session.Clear();
                Response.Cookies.Delete("username");
                Response.Cookies.Delete("token");
                Response.Cookies.Delete("session_id");
                return RedirectToPage("/Auth/Login");
            }

            if (string.IsNullOrEmpty(FullName) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                Message = "Tüm alanları doldurun.";
                Instructors = _context.Users.Where(u => u.Role == "Instructor").ToList();
                return Page();
            }

            var pepper = _configuration["Security:Pepper"];
            var passwordWithPepper = Password + pepper;
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(passwordWithPepper);

            if (EditingId.HasValue)
            {
                var instructorToUpdate = await _context.Users.FindAsync(EditingId.Value);
                if (instructorToUpdate != null && instructorToUpdate.Role == "Instructor")
                {
                    instructorToUpdate.FullName = FullName;
                    instructorToUpdate.Email = Email;
                    instructorToUpdate.PasswordHash = hashedPassword;

                    await _context.SaveChangesAsync();
                    Message = "Instructor başarıyla güncellendi.";
                    Instructors = _context.Users.Where(u => u.Role == "Instructor").ToList();
                }
            }
            else
            {
                var existing = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email);
                if (existing != null)
                {
                    Message = "Bu email ile zaten bir kullanıcı var.";
                    Instructors = _context.Users.Where(u => u.Role == "Instructor").ToList();
                    return Page();
                }

                var newInstructor = new User
                {
                    FullName = FullName,
                    Email = Email,
                    PasswordHash = hashedPassword,
                    Role = "Instructor"
                };

                _context.Users.Add(newInstructor);
                await _context.SaveChangesAsync();

                Message = "Instructor başarıyla eklendi.";
                Instructors = _context.Users.Where(u => u.Role == "Instructor").ToList();
            }
            return Page();
        }
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var instructor = await _context.Users.FindAsync(id);
            if (instructor != null && instructor.Role == "Instructor")
            {
                _context.Users.Remove(instructor);
                await _context.SaveChangesAsync();
                Message = "Instructor başarıyla silindi.";
            }
            Instructors = _context.Users.Where(u => u.Role == "Instructor").ToList();
            return Page();
        }

    }
}