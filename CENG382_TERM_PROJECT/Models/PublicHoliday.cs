using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CENG382_TERM_PROJECT.Models
{
    public class PublicHoliday
    {
        public int Id { get; set; }

        [Required]
        public int TermId { get; set; }

        [ForeignKey("TermId")]
        public Term Term { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }
    }
}
