using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using CENG382_TERM_PROJECT.Services; // Add the necessary using directive

namespace CENG382_TERM_PROJECT.Pages.Auth
{
    public class LogoutModel : PageModel
    {
        private readonly ISystemLogService _systemLogService;

        public LogoutModel(ISystemLogService systemLogService)
        {
            _systemLogService = systemLogService;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            Response.Cookies.Delete("username");
            Response.Cookies.Delete("token");
            Response.Cookies.Delete("session_id");

            // Extract userId and email for logging
            int? userId = null;
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var userIdClaim = HttpContext.User.FindFirst("UserId");
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var parsedUserId))
                {
                    userId = parsedUserId;
                }
            }

            var userEmail = User.Identity?.Name;

            // Log the logout action
            await _systemLogService.LogAsync(userId, "Logout",
                $"User with email '{userEmail}' logged out successfully.", true);

            return RedirectToPage("/Auth/Login");
        }
    }
}
