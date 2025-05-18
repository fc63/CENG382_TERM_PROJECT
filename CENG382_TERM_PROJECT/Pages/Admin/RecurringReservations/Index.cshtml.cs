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
        private readonly IEmailService _emailService;

        public IndexModel(IRecurringReservationService reservationService, IEmailService emailService)
        {
            _reservationService = reservationService;
            _emailService = emailService;
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
                ModelState.AddModelError(string.Empty, "Rezervasyon �ak��t��� i�in onaylanamad�.");
            }
            else
            {
                var reservation = await _reservationService.GetReservationByIdAsync(reservationId);
                var toEmail = reservation.Instructor.Email;
                var subject = "Rezervasyon Onay�";
                var body = $"Say�n {reservation.Instructor.FullName},\n\n" +
                           $"{reservation.Term.Name} d�neminde {reservation.Classroom.Name} s�n�f� i�in " +
                           $"{reservation.TimeSlot.DayOfWeek} g�n� saat {reservation.TimeSlot.StartTime:hh\\:mm} i�in " +
                           "yapm�� oldu�unuz rezervasyon **onaylanm��t�r**.";

                await _emailService.SendEmailAsync(toEmail, subject, body);
            }

            PendingReservations = await _reservationService.GetAllPendingReservationsAsync();
            ApprovedReservations = await _reservationService.GetAllApprovedReservationsAsync();
            return Page();
        }
        public async Task<IActionResult> OnPostCancelAsync(int reservationId)
        {
            await _reservationService.CancelApprovedReservationAsync(reservationId);

            var reservation = await _reservationService.GetReservationByIdAsync(reservationId);
            var toEmail = reservation.Instructor.Email;
            var subject = "Rezervasyon �ptal Edildi";
            var body = $"Say�n {reservation.Instructor.FullName},\n\n" +
                       $"{reservation.Term.Name} d�neminde {reservation.Classroom.Name} s�n�f� i�in " +
                       $"{reservation.TimeSlot.DayOfWeek} g�n� saat {reservation.TimeSlot.StartTime:hh\\:mm} i�in " +
                       "onaylanm�� olan rezervasyonunuz **iptal edilmi�tir**.";

            await _emailService.SendEmailAsync(toEmail, subject, body);

            PendingReservations = await _reservationService.GetAllPendingReservationsAsync();
            ApprovedReservations = await _reservationService.GetAllApprovedReservationsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostRejectAsync(int reservationId)
        {
            await _reservationService.RejectReservationAsync(reservationId);

            var reservation = await _reservationService.GetReservationByIdAsync(reservationId);
            var toEmail = reservation.Instructor.Email;
            var subject = "Rezervasyon Reddedildi";
            var body = $"Say�n {reservation.Instructor.FullName},\n\n" +
                       $"{reservation.Term.Name} d�neminde {reservation.Classroom.Name} s�n�f� i�in " +
                       $"{reservation.TimeSlot.DayOfWeek} g�n� saat {reservation.TimeSlot.StartTime:hh\\:mm} i�in " +
                       "yapm�� oldu�unuz rezervasyon **reddedilmi�tir**.";

            await _emailService.SendEmailAsync(toEmail, subject, body);

            PendingReservations = await _reservationService.GetAllPendingReservationsAsync();
            ApprovedReservations = await _reservationService.GetAllApprovedReservationsAsync();
            return Page();
        }
    }
}
