namespace CENG382_TERM_PROJECT.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public Classroom Class { get; set; }

        public int InstructorId { get; set; }
        public User Instructor { get; set; }

        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }

        public string Status { get; set; } // Pending, Approved, Rejected
        public string Reason { get; set; } // Optional: rejection reason
    }
}
