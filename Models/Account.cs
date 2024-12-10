using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int AccountNumber { get; set; }

        [ForeignKey("PrimaryUserId")]
        public int PrimaryUserId { get; set; }
        public User PrimaryAccountHolder { get; set; }

        [ForeignKey("SecondaryUserId")]
        public int SecondaryUserId { get; set; }
        public User SecondaryAccountHolder { get; set; }

        [ForeignKey("NomineeId")]
        public int NomineeId { get; set; }
        public User Nominee { get; set; }

        public DateTime CreatedDate { get; set; } 
        public DateTime UpdatedDate { get; set; } = DateTime.Now;

    }
}