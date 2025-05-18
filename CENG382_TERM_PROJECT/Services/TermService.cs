using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CENG382_TERM_PROJECT.Models;
using Microsoft.EntityFrameworkCore;

namespace CENG382_TERM_PROJECT.Services
{
    public class TermService : ITermService
    {
        private readonly AppDbContext _context;
        private readonly ISystemLogService _systemLogService;

        public TermService(AppDbContext context, ISystemLogService systemLogService)
        {
            _context = context;
            _systemLogService = systemLogService;
        }

        public async Task<List<Term>> GetAllTermsAsync()
        {
            try
            {
                var terms = await _context.Terms.ToListAsync();
                await _systemLogService.LogAsync(null, "GetAllTerms", "All terms retrieved successfully.", true);
                return terms;
            }
            catch (Exception ex)
            {
                await _systemLogService.LogAsync(null, "GetAllTerms", $"Error retrieving terms: {ex.Message}", false);
                throw;
            }
        }

        public async Task<Term> GetTermByIdAsync(int id)
        {
            try
            {
                var term = await _context.Terms.FindAsync(id);
                if (term == null)
                {
                    await _systemLogService.LogAsync(null, "GetTermById", $"Term with ID {id} not found.", false);
                    return null;
                }

                await _systemLogService.LogAsync(null, "GetTermById", $"Term with ID {id} retrieved successfully.", true);
                return term;
            }
            catch (Exception ex)
            {
                await _systemLogService.LogAsync(null, "GetTermById", $"Error retrieving term with ID {id}: {ex.Message}", false);
                throw;
            }
        }

        public async Task<bool> AddTermAsync(Term term)
        {
            try
            {
                _context.Terms.Add(term);
                await _context.SaveChangesAsync();
                await _systemLogService.LogAsync(null, "AddTerm", $"Term '{term.Name}' added successfully.", true);
                return true;
            }
            catch (Exception ex)
            {
                await _systemLogService.LogAsync(null, "AddTerm", $"Error adding term '{term.Name}': {ex.Message}", false);
                throw;
            }
        }

        public async Task<bool> UpdateTermAsync(Term term)
        {
            try
            {
                _context.Terms.Update(term);
                await _context.SaveChangesAsync();
                await _systemLogService.LogAsync(null, "UpdateTerm", $"Term '{term.Name}' with ID {term.Id} updated successfully.", true);
                return true;
            }
            catch (Exception ex)
            {
                await _systemLogService.LogAsync(null, "UpdateTerm", $"Error updating term '{term.Name}' with ID {term.Id}: {ex.Message}", false);
                throw;
            }
        }

        public async Task<bool> DeleteTermAsync(int id)
        {
            try
            {
                var term = await _context.Terms.FindAsync(id);
                if (term == null)
                {
                    await _systemLogService.LogAsync(null, "DeleteTerm", $"Term with ID {id} not found.", false);
                    return false;
                }

                _context.Terms.Remove(term);
                await _context.SaveChangesAsync();
                await _systemLogService.LogAsync(null, "DeleteTerm", $"Term '{term.Name}' with ID {id} deleted successfully.", true);
                return true;
            }
            catch (Exception ex)
            {
                await _systemLogService.LogAsync(null, "DeleteTerm", $"Error deleting term with ID {id}: {ex.Message}", false);
                throw;
            }
        }
    }
}
