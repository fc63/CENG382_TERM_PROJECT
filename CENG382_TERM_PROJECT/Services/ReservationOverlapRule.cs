using CENG382_TERM_PROJECT.Models;
using CENG382_TERM_PROJECT.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using CENG382_TERM_PROJECT.Services; // Add the necessary using directive

namespace CENG382_TERM_PROJECT.Validation
{
    public class ReservationOverlapRule : IConflictRule
    {
        private readonly AppDbContext _context;
        private readonly ISystemLogService _systemLogService;

        public ReservationOverlapRule(AppDbContext context, ISystemLogService systemLogService)
        {
            _context = context;
            _systemLogService = systemLogService;
        }

        public async Task<(bool HasConflict, string Message)> CheckConflictAsync(RecurringReservation reservation)
        {
            var reservationSlot = reservation.TimeSlot;
            if (reservationSlot == null) return (false, null);

            // Classroom conflict check
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
            {
                var message = "Bu sınıfta bu gün ve saatte zaten onaylı bir rezervasyon var.";
                await _systemLogService.LogAsync(reservation.InstructorId, "ReservationConflict",
                    $"Conflict detected in ReservationOverlapRule. ReservationId: {reservation.Id}, Message: {message}", false);
                return (true, message);
            }

            // Instructor conflict check
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
            {
                var message = "Bu eğitmenin bu saatte başka bir sınıfta onaylı dersi var.";
                await _systemLogService.LogAsync(reservation.InstructorId, "ReservationConflict",
                    $"Conflict detected in ReservationOverlapRule. ReservationId: {reservation.Id}, Message: {message}", false);
                return (true, message);
            }

            return (false, null);
        }
    }
}
