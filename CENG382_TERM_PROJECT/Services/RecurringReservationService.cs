using Microsoft.EntityFrameworkCore;
using CENG382_TERM_PROJECT.Models;

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
                    r.TimeSlotId == slotId);

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
    }
}
