using System.Collections.Generic;
using CENG382_TERM_PROJECT.Models;

namespace CENG382_TERM_PROJECT.Services
{
    public interface IPaginationService
    {
        (List<User> Instructors, int TotalPages) GetPaginatedInstructors(string searchTerm, int pageNumber, int pageSize = 10);
    }
}
