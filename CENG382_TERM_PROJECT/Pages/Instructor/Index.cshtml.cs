using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using CENG382_TERM_PROJECT.Models;
using System.Threading.Tasks;

namespace CENG382_TERM_PROJECT.Pages.Instructor
{
    [Authorize(Roles = "Instructor")]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IDataProtector _protector;

        public IndexModel(AppDbContext context, IDataProtectionProvider provider)
        {
            _context = context;
            _protector = provider.CreateProtector("CENG382_TERM_PROJECT_CookieProtector");
        }

        public IActionResult OnGet()
        {
            var sessionUsername = HttpContext.Session.GetString("username");
            var sessionToken = HttpContext.Session.GetString("token");
            var sessionId = HttpContext.Session.GetString("session_id");

            Request.Cookies.TryGetValue("username", out var protectedUsername);
            Request.Cookies.TryGetValue("token", out var protectedToken);
            Request.Cookies.TryGetValue("session_id", out var protectedSessionId);

            string cookieUsername = null;
            string cookieToken = null;
            string cookieSessionId = null;

            try
            {
                if (!string.IsNullOrEmpty(protectedUsername))
                    cookieUsername = _protector.Unprotect(protectedUsername);
            }
            catch
            {
                cookieUsername = null;
            }

            try
            {
                if (!string.IsNullOrEmpty(protectedToken))
                    cookieToken = _protector.Unprotect(protectedToken);
            }
            catch
            {
                cookieToken = null;
            }

            try
            {
                if (!string.IsNullOrEmpty(protectedSessionId))
                    cookieSessionId = _protector.Unprotect(protectedSessionId);
            }
            catch
            {
                cookieSessionId = null;
            }


            if (sessionUsername != cookieUsername || sessionToken != cookieToken || sessionId != cookieSessionId)
            {
                HttpContext.Session.Clear();
                Response.Cookies.Delete("username");
                Response.Cookies.Delete("token");
                Response.Cookies.Delete("session_id");
                return RedirectToPage("/Auth/Login");
            }

            return Page();
        }
    }
}
