using CENG382_TERM_PROJECT.Models;
using CENG382_TERM_PROJECT.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace CENG382_TERM_PROJECT.Pages.Admin.ClassroomManagement
{
    [Authorize(Roles = "Admin")]
    public class DetailsModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IFeedbackService _feedbackService;

        public DetailsModel(AppDbContext context, IFeedbackService feedbackService)
        {
            _context = context;
            _feedbackService = feedbackService;
        }

        public Classroom Classroom { get; set; }
        public List<Feedback> Feedbacks { get; set; }
        public double? AverageStars { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Classroom = await _context.Classrooms.FindAsync(id);
            if (Classroom == null)
            {
                return NotFound();
            }

            Feedbacks = await _feedbackService.GetFeedbacksByClassIdAsync(id);
            AverageStars = await _feedbackService.GetAverageStarsByClassIdAsync(id);

            return Page();
        }
    }
}
