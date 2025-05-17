using CENG382_TERM_PROJECT.Models;
using CENG382_TERM_PROJECT.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace CENG382_TERM_PROJECT.Pages.Instructor.Feedback
{
    [Authorize(Roles = "Instructor")]
    public class IndexModel : PageModel
    {
        private readonly IFeedbackService _feedbackService;
        private readonly AppDbContext _context;

        public IndexModel(IFeedbackService feedbackService, AppDbContext context)
        {
            _feedbackService = feedbackService;
            _context = context;
        }

        [BindProperty]
        public int SelectedClassId { get; set; }

        [BindProperty]
        public string Comment { get; set; }

        [BindProperty]
        public int Stars { get; set; }

        public List<Classroom> Classrooms { get; set; }

        public async Task OnGetAsync()
        {
            Classrooms = await Task.FromResult(_context.Classrooms.ToList());
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Stars < 1 || Stars > 5 || string.IsNullOrWhiteSpace(Comment))
            {
                ModelState.AddModelError("", "Yorum ve yýldýz deðeri geçerli deðil.");
                return Page();
            }

            var instructorId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            var feedback = new Models.Feedback
            {
                ClassId = SelectedClassId,
                InstructorId = instructorId,
                Comment = Comment,
                Stars = Stars
            };

            await _feedbackService.AddFeedbackAsync(feedback);
            TempData["Message"] = "Geri bildiriminiz baþarýyla gönderildi.";
            return RedirectToPage(); // Sayfayý yenile
        }
    }
}
