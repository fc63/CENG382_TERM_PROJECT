using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CENG382_TERM_PROJECT.Models;
using CENG382_TERM_PROJECT.Services;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CENG382_TERM_PROJECT.Pages.Admin.ClassroomManagement
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IClassroomService _classroomService;

        public IndexModel(IClassroomService classroomService)
        {
            _classroomService = classroomService;
        }

        public List<Classroom> Classrooms { get; set; }

        [BindProperty]
        public Classroom EditingClassroom { get; set; }

        [TempData]
        public string Message { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            Classrooms = await _classroomService.GetAllClassroomsAsync();

            if (id.HasValue)
            {
                EditingClassroom = await _classroomService.GetClassroomByIdAsync(id.Value);
                if (EditingClassroom == null)
                {
                    Message = "Classroom bulunamadı.";
                    return RedirectToPage();
                }
            }
            else
            {
                EditingClassroom = new Classroom();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Message = "Eksik veya hatalı bilgi girdiniz.";
                return RedirectToPage();
            }

            if (EditingClassroom.Id > 0)
            {
                await _classroomService.UpdateClassroomAsync(EditingClassroom);
                Message = "Classroom başarıyla güncellendi.";
            }
            else
            {
                await _classroomService.AddClassroomAsync(EditingClassroom);
                Message = "Yeni Classroom başarıyla eklendi.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _classroomService.DeleteClassroomAsync(id);
            Message = "Classroom başarıyla silindi.";
            return RedirectToPage();
        }
    }
}
