using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CENG382_TERM_PROJECT.Services
{
    public class SessionService
    {
        private readonly IDataProtector _protector;
        private readonly IMemoryCache _cache;

        public SessionService(IDataProtector protector, IMemoryCache cache)
        {
            _protector = protector;
            _cache = cache;
        }

        public IActionResult ValidateSessionAndCookies(HttpContext httpContext, PageModel page)
        {
            var sessionUsername = httpContext.Session.GetString("username");
            var sessionToken = httpContext.Session.GetString("token");
            var sessionId = httpContext.Session.GetString("session_id");

            httpContext.Request.Cookies.TryGetValue("username", out var cookieUsername);
            httpContext.Request.Cookies.TryGetValue("token", out var cookieToken);
            httpContext.Request.Cookies.TryGetValue("session_id", out var cookieSessionId);

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
                ClearSessionAndCookies(httpContext);
                return page.RedirectToPage("/Auth/Login");
            }

            var cacheToken = _cache.Get<string>(sessionUsername + "_token");

            if (cacheToken == null || cacheToken != sessionToken)
            {
                ClearSessionAndCookies(httpContext);
                return page.RedirectToPage("/Auth/Login");
            }

            return null;
        }

        public void ClearSessionAndCookies(HttpContext httpContext)
        {
            httpContext.Session.Clear();
            httpContext.Response.Cookies.Delete("username");
            httpContext.Response.Cookies.Delete("token");
            httpContext.Response.Cookies.Delete("session_id");
        }
    }
}