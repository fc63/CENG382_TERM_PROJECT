using CENG382_TERM_PROJECT.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CENG382_TERM_PROJECT.Services
{
    public class ClassroomService : IClassroomService
    {
        private readonly AppDbContext _context;

        public ClassroomService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Classroom>> GetAllClassroomsAsync()
        {
            return await _context.Classrooms.ToListAsync();
        }

        public async Task<Classroom> GetClassroomByIdAsync(int id)
        {
            return await _context.Classrooms.FindAsync(id);
        }

        public async Task<bool> AddClassroomAsync(Classroom classroom)
        {
            _context.Classrooms.Add(classroom);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateClassroomAsync(Classroom classroom)
        {
            _context.Classrooms.Update(classroom);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteClassroomAsync(int id)
        {
            var classroom = await _context.Classrooms.FindAsync(id);
            if (classroom == null)
                return false;

            _context.Classrooms.Remove(classroom);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
