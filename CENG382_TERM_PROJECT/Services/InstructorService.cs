using CENG382_TERM_PROJECT.Models;
using Microsoft.EntityFrameworkCore;

namespace CENG382_TERM_PROJECT.Services
{
    public class InstructorService : IInstructorService
    {
        private readonly AppDbContext _context;

        public InstructorService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddInstructorAsync(string fullName, string email, string hashedPassword)
        {
            var existing = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (existing != null)
            {
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
            return true;
        }

        public async Task<bool> UpdateInstructorAsync(int id, string fullName, string email, string hashedPassword)
        {
            var instructorToUpdate = await _context.Users.FindAsync(id);
            if (instructorToUpdate == null || instructorToUpdate.Role != "Instructor")
            {
                return false;
            }

            instructorToUpdate.FullName = fullName;
            instructorToUpdate.Email = email;
            instructorToUpdate.PasswordHash = hashedPassword;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteInstructorAsync(int id)
        {
            var instructor = await _context.Users.FindAsync(id);
            if (instructor == null || instructor.Role != "Instructor")
            {
                return false;
            }

            _context.Users.Remove(instructor);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
