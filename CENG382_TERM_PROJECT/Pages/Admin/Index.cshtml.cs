using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace CENG382_TERM_PROJECT.Pages.Admin
{
	[Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        public void OnGet()
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
				Response.Redirect("/Auth/Login");
				return;
			}
        }
    }
}
