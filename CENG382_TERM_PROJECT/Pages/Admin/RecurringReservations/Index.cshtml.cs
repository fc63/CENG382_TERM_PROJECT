using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using CENG382_TERM_PROJECT.Models;
using CENG382_TERM_PROJECT.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CENG382_TERM_PROJECT.Pages.Admin.RecurringReservations
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IRecurringReservationService _reservationService;

        public IndexModel(IRecurringReservationService reservationService)
        {
            _reservationService = reservationService;
        }
        public List<RecurringReservation> ApprovedReservations { get; set; } = new();
        public List<RecurringReservation> PendingReservations { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            PendingReservations = await _reservationService.GetAllPendingReservationsAsync();
            ApprovedReservations = await _reservationService.GetAllApprovedReservationsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostApproveAsync(int reservationId)
        {
            var success = await _reservationService.ApproveReservationAsync(reservationId);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Rezervasyon çakýþtýðý için onaylanamadý.");
            }

            PendingReservations = await _reservationService.GetAllPendingReservationsAsync();
            ApprovedReservations = await _reservationService.GetAllApprovedReservationsAsync();
            return Page();
        }
        public async Task<IActionResult> OnPostCancelAsync(int reservationId)
        {
            await _reservationService.CancelApprovedReservationAsync(reservationId);
            PendingReservations = await _reservationService.GetAllPendingReservationsAsync();
            ApprovedReservations = await _reservationService.GetAllApprovedReservationsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostRejectAsync(int reservationId)
        {
            await _reservationService.RejectReservationAsync(reservationId);
            PendingReservations = await _reservationService.GetAllPendingReservationsAsync();
            ApprovedReservations = await _reservationService.GetAllApprovedReservationsAsync();
            return Page();
        }
    }
}
