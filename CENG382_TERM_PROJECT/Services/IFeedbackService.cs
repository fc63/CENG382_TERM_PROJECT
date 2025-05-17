using System.Collections.Generic;
using System.Threading.Tasks;
using CENG382_TERM_PROJECT.Models;

namespace CENG382_TERM_PROJECT.Services
{
    public interface IFeedbackService
    {
        Task AddFeedbackAsync(Feedback feedback);
        Task<List<Feedback>> GetFeedbacksByClassIdAsync(int classId);
        Task<double?> GetAverageStarsByClassIdAsync(int classId);
    }
}
