using CENG382_TERM_PROJECT.Models;
using CENG382_TERM_PROJECT.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CENG382_TERM_PROJECT.Validation
{
    public class ReservationOverlapRule : IConflictRule
    {
        private readonly AppDbContext _context;

        public ReservationOverlapRule(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(bool HasConflict, string Message)> CheckConflictAsync(RecurringReservation reservation)
        {
            var reservationSlot = reservation.TimeSlot;
            if (reservationSlot == null) return (false, null);

            var conflicts = await _context.RecurringReservations
                .Where(r =>
                    r.Id != reservation.Id &&
                    r.ClassroomId == reservation.ClassroomId &&
                    r.TermId == reservation.TermId &&
                    r.Status == "Approved" &&
                    r.TimeSlot.DayOfWeek == reservationSlot.DayOfWeek &&
                    r.TimeSlot.StartTime < reservationSlot.EndTime &&
                    reservationSlot.StartTime < r.TimeSlot.EndTime
                ).AnyAsync();

            if (conflicts)
                return (true, "Bu sınıfta bu gün ve saatte zaten onaylı bir rezervasyon var.");

            return (false, null);
        }
    }
}
