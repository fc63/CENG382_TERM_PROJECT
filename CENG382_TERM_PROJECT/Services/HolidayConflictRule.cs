using CENG382_TERM_PROJECT.Models;
using CENG382_TERM_PROJECT.Data;
using CENG382_TERM_PROJECT.Services;
using System.Linq;
using System.Threading.Tasks;

namespace CENG382_TERM_PROJECT.Validation
{
    public class HolidayConflictRule : IConflictRule
    {
        private readonly IPublicHolidayService _holidayService;

        public HolidayConflictRule(IPublicHolidayService holidayService)
        {
            _holidayService = holidayService;
        }

        public async Task<(bool HasConflict, string Message)> CheckConflictAsync(RecurringReservation reservation)
        {
            var termStart = reservation.Term.StartDate;
            var termEnd = reservation.Term.EndDate;

            if (reservation.TimeSlot == null || string.IsNullOrEmpty(reservation.TimeSlot.DayOfWeek))
                return (false, null);

            var dayOfWeekEnum = Enum.Parse<DayOfWeek>(reservation.TimeSlot.DayOfWeek);
            var matchingDates = Enumerable.Range(0, (termEnd - termStart).Days + 1)
                .Select(offset => termStart.AddDays(offset))
                .Where(date => date.DayOfWeek == dayOfWeekEnum)
                .ToList();

            var holidays = await _holidayService.GetHolidaysAsync(termStart.Year);

            bool hasHoliday = matchingDates.Any(date => holidays.Any(h => h.Date == date));
            if (hasHoliday)
                return (true, "Bu zaman aralığı içinde resmi tatile denk gelen günler var.");

            return (false, null);
        }
    }
}
