using Microsoft.EntityFrameworkCore;
using CENG382_TERM_PROJECT.Models;
using CENG382_TERM_PROJECT.Validation;

namespace CENG382_TERM_PROJECT.Services
{
    public class RecurringReservationService : IRecurringReservationService
    {
        private readonly AppDbContext _context;
        private readonly ISystemLogService _systemLogService;

        public RecurringReservationService(AppDbContext context, ISystemLogService systemLogService)
        {
            _context = context;
            _systemLogService = systemLogService;
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
            try
            {
                foreach (var slotId in selectedSlotIds)
                {
                    var exists = await _context.RecurringReservations.AnyAsync(r =>
                        r.ClassroomId == classroomId &&
                        r.TermId == termId &&
                        r.TimeSlotId == slotId &&
                        r.Status == "Approved");

                    if (exists)
                    {
                        await _systemLogService.LogAsync(instructorId, "CreateRecurringReservations",
                            $"Conflict detected for InstructorId {instructorId} on SlotId {slotId}", false);
                        return false;
                    }
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
                await _systemLogService.LogAsync(instructorId, "CreateRecurringReservations",
                    $"Recurring reservations created for InstructorId {instructorId} on Slots {string.Join(", ", selectedSlotIds)}", true);
                return true;
            }
            catch (Exception ex)
            {
                await _systemLogService.LogAsync(instructorId, "CreateRecurringReservations",
                    $"Error occurred while creating reservations: {ex.Message}", false);
                throw;
            }
        }

        public async Task<bool> CancelReservationAsync(int reservationId, int instructorId)
        {
            try
            {
                var reservation = await _context.RecurringReservations
                    .FirstOrDefaultAsync(r => r.Id == reservationId && r.InstructorId == instructorId);

                if (reservation == null)
                {
                    await _systemLogService.LogAsync(instructorId, "CancelReservation",
                        $"ReservationId {reservationId} not found for InstructorId {instructorId}", false);
                    return false;
                }

                _context.RecurringReservations.Remove(reservation);
                await _context.SaveChangesAsync();
                await _systemLogService.LogAsync(instructorId, "CancelReservation",
                    $"ReservationId {reservationId} canceled for InstructorId {instructorId}", true);
                return true;
            }
            catch (Exception ex)
            {
                await _systemLogService.LogAsync(instructorId, "CancelReservation",
                    $"Error occurred while canceling reservation: {ex.Message}", false);
                throw;
            }
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
            {
                await _systemLogService.LogAsync(null, "ApproveReservation",
                    $"ReservationId {reservationId} not found or not in pending status.", false);
                return false;
            }

            var validator = new ReservationValidator(new IConflictRule[]
            {
                new ReservationOverlapRule(_context, _systemLogService)
            });

            var (isValid, message) = await validator.ValidateAsync(reservation);

            if (!isValid)
            {
                reservation.Status = "Rejected";
                reservation.Reason = message;
                await _systemLogService.LogAsync(null, "ApproveReservation",
                    $"ReservationId {reservationId} rejected due to conflict: {message}", false);
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
                    pending.Reason = "Another reservation was approved.";
                }
                reservation.Reason = null;
                await _systemLogService.LogAsync(null, "ApproveReservation",
                    $"ReservationId {reservationId} approved successfully.", true);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectReservationAsync(int reservationId, string reason)
        {
            var reservation = await _context.RecurringReservations
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null || reservation.Status != "Pending")
            {
                await _systemLogService.LogAsync(null, "RejectReservation",
                    $"ReservationId {reservationId} not found or not in pending status.", false);
                return false;
            }

            reservation.Status = "Rejected";
            reservation.Reason = string.IsNullOrWhiteSpace(reason) ? "Not specified" : reason;
            await _context.SaveChangesAsync();
            await _systemLogService.LogAsync(null, "RejectReservation",
                $"ReservationId {reservationId} was manually rejected by admin. Reason: {reservation.Reason}", true);
            return true;
        }

        public async Task<bool> RejectReservationAsync(int reservationId)
        {
            return await RejectReservationAsync(reservationId, "Not specified");
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
            {
                await _systemLogService.LogAsync(null, "CancelApprovedReservation",
                    $"ReservationId {reservationId} not found or not approved.", false);
                return false;
            }

            reservation.Status = "Rejected";
            await _context.SaveChangesAsync();
            await _systemLogService.LogAsync(null, "CancelApprovedReservation",
                $"ReservationId {reservationId} was canceled successfully.", true);
            return true;
        }

        public async Task<RecurringReservation> GetReservationByIdAsync(int reservationId)
        {
            return await _context.RecurringReservations
                .Include(r => r.Term)
                .Include(r => r.Classroom)
                .Include(r => r.TimeSlot)
                .Include(r => r.Instructor)
                .FirstOrDefaultAsync(r => r.Id == reservationId);
        }
    }
}
