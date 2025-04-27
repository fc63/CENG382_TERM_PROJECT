using CENG382_TERM_PROJECT.Models;

namespace CENG382_TERM_PROJECT.Services
{
    public class PaginationService
    {
        private readonly AppDbContext _context;

        public PaginationService(AppDbContext context)
        {
            _context = context;
        }

        public (List<User> Instructors, int TotalPages) GetPaginatedInstructors(string searchTerm, int pageNumber, int pageSize = 10)
        {
            var instructorsQuery = _context.Users.Where(u => u.Role == "Instructor").AsEnumerable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                instructorsQuery = instructorsQuery.Where(i =>
                    i.FullName.Contains(searchTerm) ||
                    (i.Email.Contains("@") && i.Email.Substring(0, i.Email.IndexOf('@')).Contains(searchTerm))
                );
            }

            int totalRecords = instructorsQuery.Count();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            var paginatedList = instructorsQuery
                .OrderBy(i => i.FullName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (paginatedList, totalPages);
        }
    }
}
