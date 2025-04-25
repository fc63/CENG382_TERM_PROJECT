using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CENG382_TERM_PROJECT.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

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

			Request.Cookies.TryGetValue("username", out var cookieUsername);
			Request.Cookies.TryGetValue("token", out var cookieToken);
			Request.Cookies.TryGetValue("session_id", out var cookieSessionId);

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
		private readonly AppDbContext _context;

		[BindProperty] public string FullName { get; set; }
		[BindProperty] public string Email { get; set; }
		[BindProperty] public string Password { get; set; }
		public string Message { get; set; }

		public IndexModel(AppDbContext context)
		{
			_context = context;
		}
		public async Task<IActionResult> OnPostAsync()
		{
			var sessionUsername = HttpContext.Session.GetString("username");
			var sessionToken = HttpContext.Session.GetString("token");
			var sessionId = HttpContext.Session.GetString("session_id");

			Request.Cookies.TryGetValue("username", out var cookieUsername);
			Request.Cookies.TryGetValue("token", out var cookieToken);
			Request.Cookies.TryGetValue("session_id", out var cookieSessionId);

			if (sessionUsername != cookieUsername || sessionToken != cookieToken || sessionId != cookieSessionId)
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
				return Page();
			}

			var existing = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email);
			if (existing != null)
			{
				Message = "Bu email ile zaten bir kullanıcı var.";
				return Page();
			}

			var hash = BCrypt.Net.BCrypt.HashPassword(Password);

			var instructor = new User
			{
				FullName = FullName,
				Email = Email,
				PasswordHash = hash,
				Role = "Instructor"
			};

			_context.Users.Add(instructor);
			await _context.SaveChangesAsync();

			Message = "Instructor başarıyla eklendi.";
			return Page();
		}
    }
}