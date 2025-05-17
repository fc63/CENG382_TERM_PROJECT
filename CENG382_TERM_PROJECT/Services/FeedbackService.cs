using CENG382_TERM_PROJECT.Models;
using Microsoft.EntityFrameworkCore;

namespace CENG382_TERM_PROJECT.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly AppDbContext _context;

        public FeedbackService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddFeedbackAsync(Feedback feedback)
        {
            feedback.Date = DateTime.Now;
            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Feedback>> GetFeedbacksByClassIdAsync(int classId)
        {
            return await _context.Feedbacks
                .Include(f => f.Instructor)
                .Where(f => f.ClassId == classId)
                .OrderByDescending(f => f.Date)
                .ToListAsync();
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
