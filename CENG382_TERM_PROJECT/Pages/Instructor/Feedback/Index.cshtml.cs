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
        private readonly IEmailService _emailService;

        public IndexModel(IFeedbackService feedbackService, AppDbContext context, IEmailService emailService)
        {
            _feedbackService = feedbackService;
            _context = context;
            _emailService = emailService;
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

            try
            {
                var adminEmail = _context.Users.FirstOrDefault(u => u.Role == "Admin")?.Email;
                var instructorName = User.FindFirst("FullName")?.Value;
                var className = _context.Classrooms.FirstOrDefault(c => c.Id == SelectedClassId)?.Name;

                if (!string.IsNullOrEmpty(adminEmail))
                {
                    var subject = "Yeni Geri Bildirim Gönderildi";
                    var body = $"Eðitmen: {instructorName}\n" +
                               $"Derslik: {className}\n" +
                               $"Yýldýz: {Stars}\n" +
                               $"Yorum: {Comment}";

                    await _emailService.SendEmailAsync(adminEmail, subject, body);
                }
            }
            catch
            {
            }

            TempData["Message"] = "Geri bildiriminiz baþarýyla gönderildi.";
            return RedirectToPage();
        }
    }
}
