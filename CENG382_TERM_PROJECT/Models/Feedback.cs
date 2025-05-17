namespace CENG382_TERM_PROJECT.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int InstructorId { get; set; }
        public int Stars { get; set; } // 1–5
        public string Comment { get; set; }
        public DateTime Date { get; set; }

        public Classroom Class { get; set; }
        public User Instructor { get; set; }
    }
}
