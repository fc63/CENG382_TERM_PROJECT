using Microsoft.EntityFrameworkCore;
using CENG382_TERM_PROJECT.Models;
using CENG382_TERM_PROJECT.Validation;

namespace CENG382_TERM_PROJECT.Services
{
    public class RecurringReservationService : IRecurringReservationService
    {
        private readonly AppDbContext _context;

        public RecurringReservationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TimeSlot>> GetAllTimeSlotsAsync()
        {
            return await _context.TimeSlots.ToListAsync();
        }

        public async Task<List<RecurringReservation>> GetInstructorReservationsAsync(int instructorId)
        {
            return await _context.RecurringReservations
                .Include(r => r.Classroom)
                .Include(r => r.Term)
                .Include(r => r.TimeSlot)
                .Where(r => r.InstructorId == instructorId)
                .ToListAsync();
        }

        public async Task<bool> CreateRecurringReservationsAsync(int instructorId, int classroomId, int termId, List<int> selectedSlotIds, string reason = "")
        {
            foreach (var slotId in selectedSlotIds)
            {
                var exists = await _context.RecurringReservations.AnyAsync(r =>
                    r.ClassroomId == classroomId &&
                    r.TermId == termId &&
                    r.TimeSlotId == slotId &&
                    r.Status == "Approved");

                if (exists)
                    return false;
            }

            foreach (var slotId in selectedSlotIds)
            {
                _context.RecurringReservations.Add(new RecurringReservation
                {
                    InstructorId = instructorId,
                    ClassroomId = classroomId,
                    TermId = termId,
                    TimeSlotId = slotId,
                    Status = "Pending",
                    Reason = reason
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> CancelReservationAsync(int reservationId, int instructorId)
        {
            var reservation = await _context.RecurringReservations
                .FirstOrDefaultAsync(r => r.Id == reservationId && r.InstructorId == instructorId);

            if (reservation == null)
                return false;

            _context.RecurringReservations.Remove(reservation);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<RecurringReservation>> GetAllPendingReservationsAsync()
        {
            return await _context.RecurringReservations
                .Include(r => r.Classroom)
                .Include(r => r.Term)
                .Include(r => r.TimeSlot)
                .Include(r => r.Instructor)
                .Where(r => r.Status == "Pending")
                .OrderBy(r => r.Term.StartDate)
                .ThenBy(r => r.TimeSlot.DayOfWeek)
                .ThenBy(r => r.TimeSlot.StartTime)
                .ToListAsync();
        }
        public async Task<bool> ApproveReservationAsync(int reservationId)
        {
            var reservation = await _context.RecurringReservations
                .Include(r => r.Classroom)
                .Include(r => r.TimeSlot)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null || reservation.Status != "Pending")
                return false;

            var validator = new ReservationValidator(new IConflictRule[]
            {
            new ReservationOverlapRule(_context)
                    });

                    var (isValid, message) = await validator.ValidateAsync(reservation);

                    if (!isValid)
                    {
                        reservation.Status = "Rejected";
                        reservation.Reason = message;
                    }
                    else
                    {
                reservation.Status = "Approved";
                var otherPendings = await _context.RecurringReservations
                    .Where(r =>
                        r.Id != reservation.Id &&
                        r.ClassroomId == reservation.ClassroomId &&
                        r.TermId == reservation.TermId &&
                        r.TimeSlotId == reservation.TimeSlotId &&
                        r.Status == "Pending")
                    .ToListAsync();
                foreach (var pending in otherPendings)
                {
                    pending.Status = "Cancelled";
                    pending.Reason = "Başka bir rezervasyon onaylandığı için iptal edildi.";
                }
                reservation.Reason = null;
                    }
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> RejectReservationAsync(int reservationId)
        {
            var reservation = await _context.RecurringReservations
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null || reservation.Status != "Pending")
                return false;

            reservation.Status = "Rejected";
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<RecurringReservation>> GetAllApprovedReservationsAsync()
        {
            return await _context.RecurringReservations
                .Include(r => r.Classroom)
                .Include(r => r.Term)
                .Include(r => r.TimeSlot)
                .Include(r => r.Instructor)
                .Where(r => r.Status == "Approved")
                .OrderBy(r => r.Term.StartDate)
                .ThenBy(r => r.TimeSlot.DayOfWeek)
                .ThenBy(r => r.TimeSlot.StartTime)
                .ToListAsync();
        }
        public async Task<bool> CancelApprovedReservationAsync(int reservationId)
        {
            var reservation = await _context.RecurringReservations.FirstOrDefaultAsync(r =>
                r.Id == reservationId && r.Status == "Approved");

            if (reservation == null)
                return false;

            reservation.Status = "Rejected";
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
