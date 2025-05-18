namespace CENG382_TERM_PROJECT.Models
{
    public class TimeSlot
    {
        public int Id { get; set; }
        public string DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
