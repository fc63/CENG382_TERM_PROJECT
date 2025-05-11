namespace CENG382_TERM_PROJECT.Models
{
    public class RecurringReservation
    {
        public int Id { get; set; }
        public int InstructorId { get; set; }
        public int ClassroomId { get; set; }
        public int TermId { get; set; }
        public int TimeSlotId { get; set; }
        public string Status { get; set; } = "Pending";
        public string? Reason { get; set; }

        public User Instructor { get; set; }
        public Classroom Classroom { get; set; }
        public Term Term { get; set; }
        public TimeSlot TimeSlot { get; set; }
    }
}
