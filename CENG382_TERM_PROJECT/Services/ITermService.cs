using System.Collections.Generic;
using System.Threading.Tasks;
using CENG382_TERM_PROJECT.Models;

namespace CENG382_TERM_PROJECT.Services
{
    public interface ITermService
    {
        Task<List<Term>> GetAllTermsAsync();
        Task<Term> GetTermByIdAsync(int id);
        Task<bool> AddTermAsync(Term term);
        Task<bool> UpdateTermAsync(Term term);
        Task<bool> DeleteTermAsync(int id);
    }
}
