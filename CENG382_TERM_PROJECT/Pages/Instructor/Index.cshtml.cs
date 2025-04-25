using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace CENG382_TERM_PROJECT.Pages.Instructor
{
	[Authorize(Roles = "Instructor")]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
