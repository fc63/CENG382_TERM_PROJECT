using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CENG382_TERM_PROJECT.Models;
using CENG382_TERM_PROJECT.Services;
using Microsoft.AspNetCore.Authorization;

namespace CENG382_TERM_PROJECT.Pages.Admin.TermManagement
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ITermService _termService;

        public IndexModel(ITermService termService)
        {
            _termService = termService;
        }

        public List<Term> Terms { get; set; }

        [BindProperty]
        public Term EditingTerm { get; set; }

        [TempData]
        public string Message { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            Terms = await _termService.GetAllTermsAsync();

            if (id.HasValue)
            {
                EditingTerm = await _termService.GetTermByIdAsync(id.Value);
                if (EditingTerm == null)
                {
                    Message = "Term bulunamadı.";
                    return RedirectToPage();
                }
            }
            else
            {
                EditingTerm = new Term();
            }

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Message = "Eksik veya hatalı bilgi girdiniz.";
                return RedirectToPage();
            }

            if (EditingTerm.Id > 0)
            {
                await _termService.UpdateTermAsync(EditingTerm);
                Message = "Term başarıyla güncellendi.";
            }
            else
            {
                await _termService.AddTermAsync(EditingTerm);
                Message = "Yeni Term başarıyla eklendi.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _termService.DeleteTermAsync(id);
            Message = "Term başarıyla silindi.";
            return RedirectToPage();
        }
    }
}
