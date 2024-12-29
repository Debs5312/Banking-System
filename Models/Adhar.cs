using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Adhar
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Range(10000000, 99999999)]
        public int Number { get; set; }

        public User AdharCardHolder { get; set; }

        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}