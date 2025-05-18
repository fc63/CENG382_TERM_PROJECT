using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CENG382_TERM_PROJECT.Models;
using CENG382_TERM_PROJECT.Services;

namespace CENG382_TERM_PROJECT.Pages
{
    public class Index1Model : PageModel
    {
        private readonly IEmailService _emailService;

        public Index1Model(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await _emailService.SendEmailAsync("furkcoban@gmail.com", "Test Mail", "Bu bir testtir.");
            return Content("Mail gönderildi.");
        }
    }
}
