using System.Collections.Generic;
using System.Threading.Tasks;
using CENG382_TERM_PROJECT.Models;
using Microsoft.EntityFrameworkCore;

namespace CENG382_TERM_PROJECT.Services
{
    public class TermService : ITermService
    {
        private readonly AppDbContext _context;

        public TermService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Term>> GetAllTermsAsync()
        {
            return await _context.Terms.ToListAsync();
        }

        public async Task<Term> GetTermByIdAsync(int id)
        {
            return await _context.Terms.FindAsync(id);
        }

        public async Task<bool> AddTermAsync(Term term)
        {
            _context.Terms.Add(term);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateTermAsync(Term term)
        {
            _context.Terms.Update(term);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTermAsync(int id)
        {
            var term = await _context.Terms.FindAsync(id);
            if (term == null)
                return false;

            _context.Terms.Remove(term);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
