using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using CENG382_TERM_PROJECT.Models;
using CENG382_TERM_PROJECT.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CENG382_TERM_PROJECT.Pages.Instructor.ReservationManagement
{
    [Authorize(Roles = "Instructor")]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IRecurringReservationService _reservationService;
        private readonly IPublicHolidayService _holidayService;

        public IndexModel(AppDbContext context, IRecurringReservationService reservationService, IPublicHolidayService holidayService)
        {
            _context = context;
            _reservationService = reservationService;
            _holidayService = holidayService;
        }
        [BindProperty]
        public int SelectedClassroomId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SelectedTermId { get; set; }

        [BindProperty]
        public List<int> SelectedTimeSlotIds { get; set; } = new();

        public List<Classroom> AvailableClassrooms { get; set; }
        public List<PublicHoliday> Holidays { get; set; } = new();
        public List<Term> AvailableTerms { get; set; }
        public List<TimeSlot> AllTimeSlots { get; set; }
        public List<RecurringReservation> MyReservations { get; set; }
        public Dictionary<(string day, TimeOnly time), RecurringReservation> WeeklyReservationMap { get; set; } = new();

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
                SelectedTermId.Value,
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
            // Tüm dönemleri çek
            AvailableTerms = _context.Terms.ToList();

            // Eğer SelectedTermId boşsa veya geçersizse ilk dönemi al
            var termIdToShow = SelectedTermId.HasValue && SelectedTermId > 0
                ? SelectedTermId.Value
                : AvailableTerms.FirstOrDefault()?.Id ?? 0;

            // Sayfa üzerindeki SelectedTermId alanını da set et (görselde aktif gözüksün)
            SelectedTermId = termIdToShow;

            // Tüm sınıflar ve saat slotlarını çek
            AvailableClassrooms = _context.Classrooms.ToList();
            AllTimeSlots = (await _reservationService.GetAllTimeSlotsAsync())
                .Where(slot => slot.StartTime >= new TimeOnly(9, 0) && slot.EndTime <= new TimeOnly(17, 0))
                .ToList();

            // Instructor ID al
            int instructorId = GetCurrentInstructorId();

            // Instructor'ın bu döneme ait rezervasyonlarını çek
            MyReservations = await _reservationService.GetInstructorReservationsAsync(instructorId);
            MyReservations = MyReservations
                .Where(r => r.TermId == termIdToShow && r.Status != "Rejected")
                .ToList();

            // Haftalık takvimi hazırlamak için map oluştur
            WeeklyReservationMap = MyReservations
                .GroupBy(r => (r.TimeSlot.DayOfWeek, r.TimeSlot.StartTime))
                .ToDictionary(
                    g => g.Key,
                    g => g.First()
                );
            var term = await _context.Terms.FirstOrDefaultAsync(t => t.Id == SelectedTermId);
            if (term != null)
            {
                Holidays = await _holidayService.GetOrFetchHolidaysByTermAsync(term.Id, term.StartDate, term.EndDate);
            }
        }

        private int GetCurrentInstructorId()
        {
            var email = User.Identity?.Name;
            return _context.Users.FirstOrDefault(u => u.Email == email)?.Id ?? 0;
        }
        public async Task<IActionResult> OnPostCancelAsync(int reservationId)
        {
            int instructorId = GetCurrentInstructorId();
            await _reservationService.CancelReservationAsync(reservationId, instructorId);
            await LoadDataAsync();
            return RedirectToPage();
        }
    }
}
