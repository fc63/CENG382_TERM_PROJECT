using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace CENG382_TERM_PROJECT.Services
{
    public interface ISessionService
    {
        Task<IActionResult> ValidateSessionAndCookies(HttpContext httpContext, PageModel page);
        void ClearSessionAndCookies(HttpContext httpContext);
    }
}
