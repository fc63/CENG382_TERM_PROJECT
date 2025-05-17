using CENG382_TERM_PROJECT.Models;
using CENG382_TERM_PROJECT.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace CENG382_TERM_PROJECT.Pages.Admin.ClassroomFeedback
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IFeedbackService _feedbackService;

        public IndexModel(AppDbContext context, IFeedbackService feedbackService)
        {
            _context = context;
            _feedbackService = feedbackService;
        }

        public List<Classroom> Classrooms { get; set; }
        public Dictionary<int, List<Feedback>> FeedbacksByClassId { get; set; }
        public Dictionary<int, double?> AverageStarsByClassId { get; set; }

        public async Task OnGetAsync()
        {
            Classrooms = _context.Classrooms.ToList();
            FeedbacksByClassId = new();
            AverageStarsByClassId = new();

            foreach (var classroom in Classrooms)
            {
                var feedbacks = await _feedbackService.GetFeedbacksByClassIdAsync(classroom.Id);
                FeedbacksByClassId[classroom.Id] = feedbacks;
                AverageStarsByClassId[classroom.Id] = await _feedbackService.GetAverageStarsByClassIdAsync(classroom.Id);
            }
        }
    }
}
