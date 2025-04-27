using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CENG382_TERM_PROJECT.Models;
using CENG382_TERM_PROJECT.Services;

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

            int pageSize = 10;
            RefreshPagination();
            return Page();
        }
        private readonly ISessionService _sessionService;
        private readonly IInstructorService _instructorService;
        private readonly IPaginationService _paginationService;
        private readonly IPasswordService _passwordService;

        [BindProperty] public string FullName { get; set; }
		[BindProperty] public string Email { get; set; }
		[BindProperty] public string Password { get; set; }
        [BindProperty] public int? EditingId { get; set; }
        public string Message { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public List<User> PaginatedInstructors { get; set; }
        public IndexModel(ISessionService sessionService, IInstructorService instructorService, IPaginationService paginationService, IPasswordService passwordService)
        {
            _sessionService = sessionService;
            _instructorService = instructorService;
            _paginationService = paginationService;
            _passwordService = passwordService;
        }
        private void RefreshPagination()
        {
            (var paginatedInstructors, var totalPages) = _paginationService.GetPaginatedInstructors(SearchTerm, PageNumber);
            PaginatedInstructors = paginatedInstructors;
            TotalPages = totalPages;
            CurrentPage = PageNumber;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var validationResult = _sessionService.ValidateSessionAndCookies(HttpContext, this);
            if (validationResult != null)
                return validationResult;

            if (string.IsNullOrEmpty(FullName) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                Message = "Tüm alanları doldurun.";
                RefreshPagination();
                return RedirectToPage(new { showForm = true });
            }

            var hashedPassword = _passwordService.HashPassword(Password);

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

    var deleteResult = await _instructorService.DeleteInstructorAsync(id);
    if (deleteResult)
    {
        Message = "Instructor başarıyla silindi.";
    }
    else
    {
        Message = "Silme işlemi başarısız.";
    }
            RefreshPagination();
            return RedirectToPage(new { showList = true });
        }
    }
}