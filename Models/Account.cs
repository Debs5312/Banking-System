using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Account
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public int AccountNumber { get; set; }


        [Required]
        [ForeignKey("PrimaryUserId")]
        public Guid PrimaryUserId { get; set; }
        public User PrimaryAccountHolder { get; set; }

        [ForeignKey("SecondaryUserId")]
        public Guid? SecondaryUserId { get; set; }
        public User SecondaryAccountHolder { get; set; }

        [ForeignKey("NomineeId")]
        public Guid? NomineeId { get; set; }
        public User Nominee { get; set; }

        public DateTime CreatedDate { get; set; } 
        public DateTime UpdatedDate { get; set; } = DateTime.Now;

    }
}