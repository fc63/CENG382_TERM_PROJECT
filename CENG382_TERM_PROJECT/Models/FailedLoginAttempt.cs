namespace CENG382_TERM_PROJECT.Models
{
    public class FailedLoginAttempt
    {
        public int Id { get; set; }
        public string IPAddress { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public int AttemptCount { get; set; }
        public DateTime LastAttemptTime { get; set; }
        public DateTime? BannedUntil { get; set; }
    }
}
