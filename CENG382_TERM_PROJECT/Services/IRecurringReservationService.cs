using System.Collections.Generic;
using System.Threading.Tasks;
using CENG382_TERM_PROJECT.Models;

namespace CENG382_TERM_PROJECT.Services
{
    public interface IRecurringReservationService
    {
        Task<List<TimeSlot>> GetAllTimeSlotsAsync();
        Task<List<RecurringReservation>> GetInstructorReservationsAsync(int instructorId);
        Task<bool> CreateRecurringReservationsAsync(int instructorId, int classroomId, int termId, List<int> selectedSlotIds, string reason = "");
        Task<bool> CancelReservationAsync(int reservationId, int instructorId);
        Task<List<RecurringReservation>> GetAllPendingReservationsAsync();
        Task<bool> ApproveReservationAsync(int reservationId);
        Task<bool> RejectReservationAsync(int reservationId);
        Task<List<RecurringReservation>> GetAllApprovedReservationsAsync();
        Task<bool> CancelApprovedReservationAsync(int reservationId);
        Task<RecurringReservation> GetReservationByIdAsync(int reservationId);
    }
}
