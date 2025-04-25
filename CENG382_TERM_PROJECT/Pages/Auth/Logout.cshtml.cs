using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;

namespace CENG382_TERM_PROJECT.Pages.Auth
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
			HttpContext.Session.Clear();
			Response.Cookies.Delete("username");
			Response.Cookies.Delete("token");
			Response.Cookies.Delete("session_id");
			await HttpContext.SignOutAsync();
			return RedirectToPage("/Auth/Login");
        }
    }
}
