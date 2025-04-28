using System.ComponentModel.DataAnnotations;

namespace CENG382_TERM_PROJECT.Models
{
    public class Term
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Term adı zorunludur.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Başlangıç tarihi zorunludur.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Bitiş tarihi zorunludur.")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
    }
}
