using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using CENG382_TERM_PROJECT.Models;
using CENG382_TERM_PROJECT.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace CENG382_TERM_PROJECT.Pages.Instructor.ReservationManagement
{
    [Authorize(Roles = "Instructor")]
    public class IndexModel : PageModel
    {
        private readonly IReservationService _reservationService;
        private readonly IInstructorService _instructorService;
        private readonly AppDbContext _context;

        public IndexModel(IReservationService reservationService, IInstructorService instructorService, AppDbContext context)
        {
            _reservationService = reservationService;
            _instructorService = instructorService;
            _context = context;
        }

        [BindProperty]
        public int ClassId { get; set; }

        [BindProperty]
        public DateTime StartDateTime { get; set; }

        [BindProperty]
        public DateTime EndDateTime { get; set; }

        public List<Classroom> AvailableClassrooms { get; set; }
        public List<Reservation> MyReservations { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            int instructorId = GetCurrentInstructorId();
            AvailableClassrooms = _context.Classrooms.ToList();
            MyReservations = await _reservationService.GetReservationsByInstructorIdAsync(instructorId);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return await OnGetAsync();
            }

            if (StartDateTime < DateTime.Now)
            {
                ModelState.AddModelError(string.Empty, "Başlangıç zamanı geçmiş bir tarih olamaz.");
                return await OnGetAsync();
            }

            if (StartDateTime >= EndDateTime)
            {
                ModelState.AddModelError(string.Empty, "Bitiş zamanı başlangıç zamanından sonra olmalıdır.");
                return await OnGetAsync();
            }

            int instructorId = GetCurrentInstructorId();

            var newReservation = new Reservation
            {
                ClassId = ClassId,
                InstructorId = instructorId,
                StartDateTime = StartDateTime,
                EndDateTime = EndDateTime,
                Status = "Pending",
                Reason = ""
            };

            await _reservationService.AddReservationAsync(newReservation);
            return RedirectToPage();
        }

        private int GetCurrentInstructorId()
        {
            var email = User.Identity?.Name;
            var instructor = _context.Users.FirstOrDefault(u => u.Email == email && u.Role == "Instructor");
            return instructor?.Id ?? 0;
        }
    }
}
