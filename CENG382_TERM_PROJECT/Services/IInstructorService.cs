using System.Threading.Tasks;

namespace CENG382_TERM_PROJECT.Services
{
    public interface IInstructorService
    {
        Task<bool> AddInstructorAsync(string fullName, string email, string hashedPassword);
        Task<bool> UpdateInstructorAsync(int id, string fullName, string email, string hashedPassword);
        Task<bool> DeleteInstructorAsync(int id);
    }
}
