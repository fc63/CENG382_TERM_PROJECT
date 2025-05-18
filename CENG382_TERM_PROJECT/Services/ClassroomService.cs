using CENG382_TERM_PROJECT.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CENG382_TERM_PROJECT.Services
{
    public class ClassroomService : IClassroomService
    {
        private readonly AppDbContext _context;
        private readonly ISystemLogService _systemLogService;

        public ClassroomService(AppDbContext context, ISystemLogService systemLogService)
        {
            _context = context;
            _systemLogService = systemLogService;
        }

        public async Task<List<Classroom>> GetAllClassroomsAsync()
        {
            try
            {
                var classrooms = await _context.Classrooms.ToListAsync();
                await _systemLogService.LogAsync(null, "GetAllClassrooms", "All classrooms retrieved successfully.", true);
                return classrooms;
            }
            catch (Exception ex)
            {
                await _systemLogService.LogAsync(null, "GetAllClassrooms", $"Error retrieving classrooms: {ex.Message}", false);
                throw;
            }
        }

        public async Task<Classroom> GetClassroomByIdAsync(int id)
        {
            try
            {
                var classroom = await _context.Classrooms.FindAsync(id);
                if (classroom == null)
                {
                    await _systemLogService.LogAsync(null, "GetClassroomById", $"Classroom with ID {id} not found.", false);
                    return null;
                }

                await _systemLogService.LogAsync(null, "GetClassroomById", $"Classroom with ID {id} retrieved successfully.", true);
                return classroom;
            }
            catch (Exception ex)
            {
                await _systemLogService.LogAsync(null, "GetClassroomById", $"Error retrieving classroom with ID {id}: {ex.Message}", false);
                throw;
            }
        }

        public async Task<bool> AddClassroomAsync(Classroom classroom)
        {
            try
            {
                _context.Classrooms.Add(classroom);
                await _context.SaveChangesAsync();
                await _systemLogService.LogAsync(null, "AddClassroom", $"Classroom '{classroom.Name}' added successfully.", true);
                return true;
            }
            catch (Exception ex)
            {
                await _systemLogService.LogAsync(null, "AddClassroom", $"Error adding classroom '{classroom.Name}': {ex.Message}", false);
                throw;
            }
        }

        public async Task<bool> UpdateClassroomAsync(Classroom classroom)
        {
            try
            {
                _context.Classrooms.Update(classroom);
                await _context.SaveChangesAsync();
                await _systemLogService.LogAsync(null, "UpdateClassroom", $"Classroom '{classroom.Name}' with ID {classroom.Id} updated successfully.", true);
                return true;
            }
            catch (Exception ex)
            {
                await _systemLogService.LogAsync(null, "UpdateClassroom", $"Error updating classroom '{classroom.Name}' with ID {classroom.Id}: {ex.Message}", false);
                throw;
            }
        }

        public async Task<bool> DeleteClassroomAsync(int id)
        {
            try
            {
                var classroom = await _context.Classrooms.FindAsync(id);
                if (classroom == null)
                {
                    await _systemLogService.LogAsync(null, "DeleteClassroom", $"Classroom with ID {id} not found.", false);
                    return false;
                }

                _context.Classrooms.Remove(classroom);
                await _context.SaveChangesAsync();
                await _systemLogService.LogAsync(null, "DeleteClassroom", $"Classroom '{classroom.Name}' with ID {id} deleted successfully.", true);
                return true;
            }
            catch (Exception ex)
            {
                await _systemLogService.LogAsync(null, "DeleteClassroom", $"Error deleting classroom with ID {id}: {ex.Message}", false);
                throw;
            }
        }
    }
}
