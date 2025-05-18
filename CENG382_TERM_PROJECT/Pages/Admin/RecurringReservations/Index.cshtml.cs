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
                ModelState.AddModelError(string.Empty, "Rezervasyon çakýþtýðý için onaylanamadý.");
            }
            else
            {
                var reservation = await _reservationService.GetReservationByIdAsync(reservationId);
                var toEmail = reservation.Instructor.Email;
                var subject = "Rezervasyon Onayý";
                var body = $"Sayýn {reservation.Instructor.FullName},\n\n" +
                           $"{reservation.Term.Name} döneminde {reservation.Classroom.Name} sýnýfý için " +
                           $"{reservation.TimeSlot.DayOfWeek} günü saat {reservation.TimeSlot.StartTime:hh\\:mm} için " +
                           "yapmýþ olduðunuz rezervasyon **onaylanmýþtýr**.";

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
            var subject = "Rezervasyon Ýptal Edildi";
            var body = $"Sayýn {reservation.Instructor.FullName},\n\n" +
                       $"{reservation.Term.Name} döneminde {reservation.Classroom.Name} sýnýfý için " +
                       $"{reservation.TimeSlot.DayOfWeek} günü saat {reservation.TimeSlot.StartTime:hh\\:mm} için " +
                       "onaylanmýþ olan rezervasyonunuz **iptal edilmiþtir**.";

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
            var body = $"Sayýn {reservation.Instructor.FullName},\n\n" +
                       $"{reservation.Term.Name} döneminde {reservation.Classroom.Name} sýnýfý için " +
                       $"{reservation.TimeSlot.DayOfWeek} günü saat {reservation.TimeSlot.StartTime:hh\\:mm} için " +
                       "yapmýþ olduðunuz rezervasyon **reddedilmiþtir**.";

            await _emailService.SendEmailAsync(toEmail, subject, body);

            PendingReservations = await _reservationService.GetAllPendingReservationsAsync();
            ApprovedReservations = await _reservationService.GetAllApprovedReservationsAsync();
            return Page();
        }
    }
}
