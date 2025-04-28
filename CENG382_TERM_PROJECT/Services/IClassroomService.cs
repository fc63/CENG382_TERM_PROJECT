using System.Collections.Generic;
using System.Threading.Tasks;
using CENG382_TERM_PROJECT.Models;

namespace CENG382_TERM_PROJECT.Services
{
    public interface IClassroomService
    {
        Task<List<Classroom>> GetAllClassroomsAsync();
        Task<Classroom> GetClassroomByIdAsync(int id);
        Task<bool> AddClassroomAsync(Classroom classroom);
        Task<bool> UpdateClassroomAsync(Classroom classroom);
        Task<bool> DeleteClassroomAsync(int id);
    }
}
