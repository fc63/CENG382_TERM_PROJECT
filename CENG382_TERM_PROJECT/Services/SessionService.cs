using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CENG382_TERM_PROJECT.Services
{
    public class SessionService : ISessionService
    {
        private readonly IDataProtector _protector;
        private readonly IMemoryCache _cache;
        private readonly ISystemLogService _systemLogService;

        public SessionService(IDataProtectionProvider provider, IMemoryCache cache, ISystemLogService systemLogService)
        {
            _protector = provider.CreateProtector("CENG382_TERM_PROJECT_CookieProtector");
            _cache = cache;
            _systemLogService = systemLogService;
        }

        public async Task<IActionResult> ValidateSessionAndCookies(HttpContext httpContext, PageModel page)
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

            try { if (!string.IsNullOrEmpty(cookieUsername)) decryptedUsername = _protector.Unprotect(cookieUsername); } catch { decryptedUsername = null; }
            try { if (!string.IsNullOrEmpty(cookieToken)) decryptedToken = _protector.Unprotect(cookieToken); } catch { decryptedToken = null; }
            try { if (!string.IsNullOrEmpty(cookieSessionId)) decryptedSessionId = _protector.Unprotect(cookieSessionId); } catch { decryptedSessionId = null; }

            int? userId = null;
            if (int.TryParse(decryptedUsername, out var parsedUserId)) userId = parsedUserId;

            if (sessionUsername != decryptedUsername || sessionToken != decryptedToken || sessionId != decryptedSessionId)
            {
                await _systemLogService.LogAsync(userId, "SessionCookieMismatch",
                    $"Session and cookie data mismatch for username: {decryptedUsername}.", false);
                ClearSessionAndCookies(httpContext);
                return page.RedirectToPage("/Auth/Login");
            }

            var cacheToken = _cache.Get<string>(sessionUsername + "_token");

            if (cacheToken == null || cacheToken != sessionToken)
            {
                await _systemLogService.LogAsync(userId, "SessionCacheMismatch",
                    $"Cache token mismatch or missing for username: {decryptedUsername}.", false);
                ClearSessionAndCookies(httpContext);
                return page.RedirectToPage("/Auth/Login");
            }

            await _systemLogService.LogAsync(userId, "SessionValidated",
                $"Session validated successfully for username: {decryptedUsername}.", true);

            return null;
        }

        public void ClearSessionAndCookies(HttpContext httpContext)
        {
            var username = httpContext.Session.GetString("username");

            // username'ý int? türüne dönüþtür
            int? userId = null;
            if (int.TryParse(username, out var parsedUserId))
            {
                userId = parsedUserId;
            }

            httpContext.Session.Clear();
            httpContext.Response.Cookies.Delete("username");
            httpContext.Response.Cookies.Delete("token");
            httpContext.Response.Cookies.Delete("session_id");

            _systemLogService.LogAsync(userId, "SessionCleared",
                $"Session and cookies cleared for username: {username}.", true).Wait();
        }
    }
}