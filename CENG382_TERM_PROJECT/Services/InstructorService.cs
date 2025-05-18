using CENG382_TERM_PROJECT.Models;
using Microsoft.EntityFrameworkCore;

namespace CENG382_TERM_PROJECT.Services
{
    public class InstructorService : IInstructorService
    {
        private readonly AppDbContext _context;
        private readonly ISystemLogService _systemLogService;

        public InstructorService(AppDbContext context, ISystemLogService systemLogService)
        {
            _context = context;
            _systemLogService = systemLogService;
        }

        public async Task<bool> AddInstructorAsync(string fullName, string email, string hashedPassword)
        {
            try
            {
                var existing = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (existing != null)
                {
                    await _systemLogService.LogAsync(null, "AddInstructor",
                        $"Failed to add instructor. Email {email} already exists.", false);
                    return false;
                }

                var newInstructor = new User
                {
                    FullName = fullName,
                    Email = email,
                    PasswordHash = hashedPassword,
                    Role = "Instructor"
                };

                _context.Users.Add(newInstructor);
                await _context.SaveChangesAsync();

                await _systemLogService.LogAsync(newInstructor.Id, "AddInstructor",
                    $"Instructor added successfully. FullName: {fullName}, Email: {email}", true);
                return true;
            }
            catch (Exception ex)
            {
                await _systemLogService.LogAsync(null, "AddInstructor",
                    $"Error occurred while adding instructor. FullName: {fullName}, Email: {email}. Error: {ex.Message}", false);
                throw;
            }
        }

        public async Task<bool> UpdateInstructorAsync(int id, string fullName, string email, string hashedPassword)
        {
            try
            {
                var instructorToUpdate = await _context.Users.FindAsync(id);
                if (instructorToUpdate == null || instructorToUpdate.Role != "Instructor")
                {
                    await _systemLogService.LogAsync(id, "UpdateInstructor",
                        $"Failed to update instructor. InstructorId {id} not found or not an instructor.", false);
                    return false;
                }

                instructorToUpdate.FullName = fullName;
                instructorToUpdate.Email = email;
                instructorToUpdate.PasswordHash = hashedPassword;

                await _context.SaveChangesAsync();

                await _systemLogService.LogAsync(id, "UpdateInstructor",
                    $"Instructor updated successfully. InstructorId: {id}, FullName: {fullName}, Email: {email}", true);
                return true;
            }
            catch (Exception ex)
            {
                await _systemLogService.LogAsync(id, "UpdateInstructor",
                    $"Error occurred while updating instructor. InstructorId: {id}, FullName: {fullName}, Email: {email}. Error: {ex.Message}", false);
                throw;
            }
        }

        public async Task<bool> DeleteInstructorAsync(int id)
        {
            try
            {
                var instructor = await _context.Users.FindAsync(id);
                if (instructor == null || instructor.Role != "Instructor")
                {
                    await _systemLogService.LogAsync(id, "DeleteInstructor",
                        $"Failed to delete instructor. InstructorId {id} not found or not an instructor.", false);
                    return false;
                }

                _context.Users.Remove(instructor);
                await _context.SaveChangesAsync();

                await _systemLogService.LogAsync(id, "DeleteInstructor",
                    $"Instructor deleted successfully. InstructorId: {id}, FullName: {instructor.FullName}, Email: {instructor.Email}", true);
                return true;
            }
            catch (Exception ex)
            {
                await _systemLogService.LogAsync(id, "DeleteInstructor",
                    $"Error occurred while deleting instructor. InstructorId: {id}. Error: {ex.Message}", false);
                throw;
            }
        }
    }
}
