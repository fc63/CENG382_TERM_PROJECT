using CENG382_TERM_PROJECT.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CENG382_TERM_PROJECT.Services
{
    public class PublicHolidayService : IPublicHolidayService
    {
        private readonly AppDbContext _context;

        public PublicHolidayService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PublicHoliday>> GetHolidaysByTermAsync(int termId)
        {
            return await _context.PublicHolidays
                .Where(h => h.TermId == termId)
                .OrderBy(h => h.Date)
                .ToListAsync();
        }

        public async Task<List<PublicHoliday>> GetAllAsync()
        {
            return await _context.PublicHolidays
                .Include(h => h.Term)
                .OrderBy(h => h.Date)
                .ToListAsync();
        }
        private readonly PublicHolidayApiClient _apiClient;

        public PublicHolidayService(AppDbContext context, PublicHolidayApiClient apiClient)
        {
            _context = context;
            _apiClient = apiClient;
        }
        public async Task<List<PublicHoliday>> GetOrFetchHolidaysByTermAsync(int termId, DateTime start, DateTime end)
        {
            var existing = await _context.PublicHolidays
                .Where(h => h.TermId == termId)
                .ToListAsync();

            if (existing.Count > 0)
                return existing;

            var fetched = await _apiClient.FetchHolidaysAsync(start.Year);
            var filtered = fetched
                .Where(d => d.date.ToDateTime(TimeOnly.MinValue) >= start && d.date.ToDateTime(TimeOnly.MinValue) <= end)
                .ToList();

            var newEntities = filtered.Select(x => new PublicHoliday
            {
                TermId = termId,
                Date = x.date,
                Description = x.description
            }).ToList();

            _context.PublicHolidays.AddRange(newEntities);
            await _context.SaveChangesAsync();

            return newEntities;
        }
    }
}
