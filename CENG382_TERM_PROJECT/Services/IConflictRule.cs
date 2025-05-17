using System.Threading.Tasks;
using CENG382_TERM_PROJECT.Models;
using CENG382_TERM_PROJECT.Data;

namespace CENG382_TERM_PROJECT.Validation
{
    public interface IConflictRule
    {
        Task<(bool HasConflict, string Message)> CheckConflictAsync(RecurringReservation reservation);
    }
}
