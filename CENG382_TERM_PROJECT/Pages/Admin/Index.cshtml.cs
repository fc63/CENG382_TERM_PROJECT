using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CENG382_TERM_PROJECT.Models;
using CENG382_TERM_PROJECT.Services;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Memory;

namespace CENG382_TERM_PROJECT.Pages.Admin
{
	[Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public bool ShowList { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool ShowForm { get; set; }
        public IActionResult OnGet()
        {
            var validationResult = _sessionService.ValidateSessionAndCookies(HttpContext, this);
            if (validationResult != null)
                return validationResult;

            ViewData["ShowList"] = ShowList;
            ViewData["ShowForm"] = ShowForm;

            int pageSize = 10;
            (var instructors, var totalPages) = _paginationService.GetPaginatedInstructors(SearchTerm, PageNumber);
            PaginatedInstructors = instructors;
            TotalPages = totalPages;
            CurrentPage = PageNumber;
            return Page();
        }
        private readonly AppDbContext _context;
        private readonly IDataProtector _protector;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private readonly SessionService _sessionService;
        private readonly InstructorService _instructorService;
        private readonly PaginationService _paginationService;

        [BindProperty] public string FullName { get; set; }
		[BindProperty] public string Email { get; set; }
		[BindProperty] public string Password { get; set; }
        [BindProperty] public int? EditingId { get; set; }
        public string Message { get; set; }
        public List<User> Instructors { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public List<User> PaginatedInstructors { get; set; }
        public IndexModel(AppDbContext context, IDataProtectionProvider provider, IConfiguration configuration, IMemoryCache cache)
        {
            _context = context;
            _protector = provider.CreateProtector("CENG382_TERM_PROJECT_CookieProtector");
            _configuration = configuration;
            _cache = cache;
            _sessionService = new SessionService(_protector, _cache);
            _instructorService = new InstructorService(context);
            _paginationService = new PaginationService(context);
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var validationResult = _sessionService.ValidateSessionAndCookies(HttpContext, this);
            if (validationResult != null)
                return validationResult;

            IQueryable<User> instructorsQuery = null;
            int totalRecords = 0;

            if (string.IsNullOrEmpty(FullName) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                Message = "Tüm alanları doldurun.";
                (var instructors, var totalPages) = _paginationService.GetPaginatedInstructors(SearchTerm, PageNumber);
                PaginatedInstructors = instructors;
                TotalPages = totalPages;
                CurrentPage = PageNumber;
                return RedirectToPage(new { showForm = true });
            }

            var pepper = _configuration["Security:Pepper"];
            var passwordWithPepper = Password + pepper;
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(passwordWithPepper);

            if (EditingId.HasValue)
            {
                var updateResult = await _instructorService.UpdateInstructorAsync(EditingId.Value, FullName, Email, hashedPassword);
                if (updateResult)
                {
                    Message = "Instructor başarıyla güncellendi.";
                    return RedirectToPage(new { showList = true });
                }
                else
                {
                    Message = "Güncelleme başarısız oldu.";
                    return RedirectToPage(new { showForm = true });
                }
            }
            else
            {
                var addResult = await _instructorService.AddInstructorAsync(FullName, Email, hashedPassword);
                if (addResult)
                {
                    Message = "Instructor başarıyla eklendi.";
                    return RedirectToPage(new { showForm = true });
                }
                else
                {
                    Message = "Bu email ile zaten bir kullanıcı var.";
                    return RedirectToPage(new { showForm = true });
                }
            }
        }
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var validationResult = _sessionService.ValidateSessionAndCookies(HttpContext, this);
            if (validationResult != null)
                return validationResult;

            IQueryable<User> instructorsQuery = null;
            int totalRecords = 0;
    var deleteResult = await _instructorService.DeleteInstructorAsync(id);
    if (deleteResult)
    {
        Message = "Instructor başarıyla silindi.";
    }
    else
    {
        Message = "Silme işlemi başarısız.";
    }
            (var instructors, var totalPages) = _paginationService.GetPaginatedInstructors(SearchTerm, PageNumber);
            PaginatedInstructors = instructors;
            TotalPages = totalPages;
            CurrentPage = PageNumber;
            return RedirectToPage(new { showList = true });
        }
    }
}