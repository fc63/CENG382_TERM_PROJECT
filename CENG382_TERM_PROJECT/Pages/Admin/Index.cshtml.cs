using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CENG382_TERM_PROJECT.Models;
using CENG382_TERM_PROJECT.Services;

namespace CENG382_TERM_PROJECT.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ISessionService _sessionService;

        public string Username { get; set; }

        public IndexModel(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public IActionResult OnGet()
        {
            var validationResult = _sessionService.ValidateSessionAndCookies(HttpContext, this);
            if (validationResult != null)
                return validationResult;

            Username = HttpContext.Session.GetString("username");
            return Page();
        }
    }
}
