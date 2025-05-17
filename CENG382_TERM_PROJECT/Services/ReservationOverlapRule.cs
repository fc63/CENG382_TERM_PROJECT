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

            // aynı sınıfta çakışma kontrolü
            var classroomConflict = await _context.RecurringReservations
                .Where(r =>
                    r.Id != reservation.Id &&
                    r.ClassroomId == reservation.ClassroomId &&
                    r.TermId == reservation.TermId &&
                    r.Status == "Approved" &&
                    r.TimeSlot.DayOfWeek == reservationSlot.DayOfWeek &&
                    r.TimeSlot.StartTime < reservationSlot.EndTime &&
                    reservationSlot.StartTime < r.TimeSlot.EndTime
                ).AnyAsync();

            if (classroomConflict)
                return (true, "Bu sınıfta bu gün ve saatte zaten onaylı bir rezervasyon var.");

            // instructor çakışma kontrolü
            var instructorConflict = await _context.RecurringReservations
                .Where(r =>
                    r.Id != reservation.Id &&
                    r.InstructorId == reservation.InstructorId &&
                    r.TermId == reservation.TermId &&
                    r.Status == "Approved" &&
                    r.TimeSlot.DayOfWeek == reservationSlot.DayOfWeek &&
                    r.TimeSlot.StartTime < reservationSlot.EndTime &&
                    reservationSlot.StartTime < r.TimeSlot.EndTime
                ).AnyAsync();

            if (instructorConflict)
                return (true, "Bu eğitmenin bu saatte başka bir sınıfta onaylı dersi var.");

            return (false, null);
        }
    }
}
