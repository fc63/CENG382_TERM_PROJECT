using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using CENG382_TERM_PROJECT.Models;
using CENG382_TERM_PROJECT.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CENG382_TERM_PROJECT.Pages.Admin.TermHolidays
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ITermService _termService;
        private readonly IPublicHolidayService _holidayService;

        public IndexModel(ITermService termService, IPublicHolidayService holidayService)
        {
            _termService = termService;
            _holidayService = holidayService;
        }

        [BindProperty(SupportsGet = true)]
        public int? SelectedTermId { get; set; }

        public List<Term> Terms { get; set; }
        public List<PublicHoliday> Holidays { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Terms = await _termService.GetAllTermsAsync();

            var termId = SelectedTermId ?? Terms.FirstOrDefault()?.Id ?? 0;

            SelectedTermId = termId;
            var selectedTerm = await _termService.GetTermByIdAsync(termId);

            if (selectedTerm == null)
            {
                Holidays = new List<PublicHoliday>();
                return Page();
            }

            Holidays = await _holidayService.GetOrFetchHolidaysByTermAsync(
                termId,
                selectedTerm.StartDate,
                selectedTerm.EndDate
            );


            return Page();
        }
    }
}
