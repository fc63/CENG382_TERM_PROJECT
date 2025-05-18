using CENG382_TERM_PROJECT.Models;
using Microsoft.EntityFrameworkCore;

namespace CENG382_TERM_PROJECT.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly AppDbContext _context;
        private readonly ISystemLogService _systemLogService;

        public FeedbackService(AppDbContext context, ISystemLogService systemLogService)
        {
            _context = context;
            _systemLogService = systemLogService;
        }

        public async Task AddFeedbackAsync(Feedback feedback)
        {
            try
            {
                feedback.Date = DateTime.Now;
                _context.Feedbacks.Add(feedback);
                await _context.SaveChangesAsync();

                await _systemLogService.LogAsync(feedback.InstructorId, "SubmitFeedback",
                    $"Feedback submitted for ClassId {feedback.ClassId} with {feedback.Stars} stars and comment: {feedback.Comment}", true);
            }
            catch (Exception ex)
            {
                await _systemLogService.LogAsync(feedback.InstructorId, "SubmitFeedback",
                    $"Failed to submit feedback for ClassId {feedback.ClassId}. Error: {ex.Message}", false);
                throw;
            }
        }

        public async Task<List<Feedback>> GetFeedbacksByClassIdAsync(int classId)
        {
            var feedbacks = await _context.Feedbacks
                .Include(f => f.Instructor)
                .Where(f => f.ClassId == classId)
                .OrderByDescending(f => f.Date)
                .ToListAsync();

            await _systemLogService.LogAsync(null, "ViewFeedback",
                $"Feedbacks viewed for ClassId {classId}. Total feedbacks: {feedbacks.Count}", true);

            return feedbacks;
        }

        public async Task<double?> GetAverageStarsByClassIdAsync(int classId)
        {
            return await _context.Feedbacks
                .Where(f => f.ClassId == classId)
                .Select(f => (double?)f.Stars)
                .AverageAsync();
        }
    }
}
