using System.Threading.Tasks;

namespace CENG382_TERM_PROJECT.Services
{
    public interface ISystemLogService
    {
        Task LogAsync(int? userId, string operationType, string description, bool success);
    }
}
