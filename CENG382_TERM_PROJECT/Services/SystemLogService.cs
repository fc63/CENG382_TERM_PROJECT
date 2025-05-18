using CENG382_TERM_PROJECT.Models;
using Microsoft.EntityFrameworkCore;

namespace CENG382_TERM_PROJECT.Services
{
    public class SystemLogService : ISystemLogService
    {
        private readonly AppDbContext _context;

        public SystemLogService(AppDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(int? userId, string operationType, string description, bool success)
        {
            var log = new SystemLog
            {
                UserId = userId,
                OperationType = operationType,
                Description = description,
                Success = success
            };

            _context.SystemLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
