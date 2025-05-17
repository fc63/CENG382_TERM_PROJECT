using Microsoft.EntityFrameworkCore;

namespace CENG382_TERM_PROJECT.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
		
		public DbSet<FailedLoginAttempt> FailedLoginAttempts { get; set; }

        public DbSet<User> Users { get; set; }
		
		public DbSet<Term> Terms { get; set; }
		
		public DbSet<Classroom> Classrooms { get; set; }

        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<RecurringReservation> RecurringReservations { get; set; }
        public DbSet<PublicHoliday> PublicHolidays { get; set; }
    }
}
