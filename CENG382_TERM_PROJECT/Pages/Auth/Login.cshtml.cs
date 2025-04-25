using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CENG382_TERM_PROJECT.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using System.Text;
using CENG382_TERM_PROJECT.Utils;

namespace CENG382_TERM_PROJECT.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _context;

        public LoginModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                ErrorMessage = "Email ve şifre gereklidir.";
                return Page();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(Password, user.PasswordHash))
            {
                ErrorMessage = "Geçersiz email veya şifre.";
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
			
			var cookieOptions = new CookieOptions
			{
				Expires = DateTime.UtcNow.AddMinutes(30),
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.Strict
			};

			var token = SecurityHelper.GenerateSecureToken(user.Email);
			var sessionId = HttpContext.Session.Id;

			HttpContext.Session.SetString("username", user.Email);
			HttpContext.Session.SetString("token", token);
			HttpContext.Session.SetString("session_id", sessionId);

			Response.Cookies.Append("username", user.Email, cookieOptions);
			Response.Cookies.Append("token", token, cookieOptions);
			Response.Cookies.Append("session_id", sessionId, cookieOptions);


            if (user.Role == "Instructor")
                return RedirectToPage("/Instructor/Index");
            else if (user.Role == "Admin")
                return RedirectToPage("/Admin/Index");
			return RedirectToPage("/Index");
        }
    }
}