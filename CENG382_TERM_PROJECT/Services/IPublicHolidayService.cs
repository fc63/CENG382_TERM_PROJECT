using CENG382_TERM_PROJECT.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CENG382_TERM_PROJECT.Services
{
    public interface IPublicHolidayService
    {
        Task<List<PublicHoliday>> GetHolidaysByTermAsync(int termId);
        Task<List<PublicHoliday>> GetAllAsync();
        Task<List<PublicHoliday>> GetOrFetchHolidaysByTermAsync(int termId, DateTime start, DateTime end);
    }
}
