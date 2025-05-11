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
        private readonly AppDbContext _context;
        private readonly IRecurringReservationService _reservationService;

        public IndexModel(AppDbContext context, IRecurringReservationService reservationService)
        {
            _context = context;
            _reservationService = reservationService;
        }

        [BindProperty]
        public int SelectedClassroomId { get; set; }

        [BindProperty]
        public int SelectedTermId { get; set; }

        [BindProperty]
        public List<int> SelectedTimeSlotIds { get; set; } = new();

        public List<Classroom> AvailableClassrooms { get; set; }
        public List<Term> AvailableTerms { get; set; }
        public List<TimeSlot> AllTimeSlots { get; set; }
        public List<RecurringReservation> MyReservations { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (SelectedClassroomId == 0 || SelectedTermId == 0 || SelectedTimeSlotIds.Count == 0)
            {
                ModelState.AddModelError("", "Tüm alanları doldurun ve en az 1 saat seçin.");
                await LoadDataAsync();
                return Page();
            }

            int instructorId = GetCurrentInstructorId();
            var success = await _reservationService.CreateRecurringReservationsAsync(
                instructorId,
                SelectedClassroomId,
                SelectedTermId,
                SelectedTimeSlotIds
            );

            if (!success)
            {
                ModelState.AddModelError("", "Seçilen zaman dilimlerinden bazıları çakışıyor.");
            }

            await LoadDataAsync();
            return Page();
        }

        private async Task LoadDataAsync()
        {
            AvailableClassrooms = _context.Classrooms.ToList();
            AvailableTerms = _context.Terms.ToList();
            AllTimeSlots = await _reservationService.GetAllTimeSlotsAsync();

            int instructorId = GetCurrentInstructorId();
            MyReservations = await _reservationService.GetInstructorReservationsAsync(instructorId);
        }

        private int GetCurrentInstructorId()
        {
            var email = User.Identity?.Name;
            return _context.Users.FirstOrDefault(u => u.Email == email)?.Id ?? 0;
        }
    }
}
