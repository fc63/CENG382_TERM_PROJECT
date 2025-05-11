using CENG382_TERM_PROJECT.Models;

namespace CENG382_TERM_PROJECT.Data.Seed
{
    public static class TimeSlotSeeder
    {
        public static void Seed(AppDbContext context)
        {
            if (context.TimeSlots.Any()) return;

            string[] days = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };

            foreach (var day in days)
            {
                for (int hour = 0; hour < 24; hour++)
                {
                    var start = new TimeOnly(hour, 0);
                    var end = start.Add(TimeSpan.FromHours(1));

                    context.TimeSlots.Add(new TimeSlot
                    {
                        DayOfWeek = day,
                        StartTime = start,
                        EndTime = end
                    });
                }
            }

            context.SaveChanges();
        }
    }
}
