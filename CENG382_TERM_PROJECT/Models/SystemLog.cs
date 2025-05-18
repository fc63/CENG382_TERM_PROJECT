namespace CENG382_TERM_PROJECT.Models
{
    public class SystemLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public int? UserId { get; set; }
        public string OperationType { get; set; }
        public string Description { get; set; }
        public bool Success { get; set; }

        public User User { get; set; }
    }
}
