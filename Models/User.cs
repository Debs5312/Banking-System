using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        public string Address { get; set; }
        public string RefreshToken { get; set; }
        [Required]
        public string Role { get; set; } = "user";
        
        [ForeignKey("AdharId")]
        public Guid AdharId { get; set; }
        public Adhar AdharNumber { get; set; }

        public Account PrimaryAccount { get; set; }
        public List<Account> SecondaryAccounts { get; set; }
        public List<Account> Nominees { get; set; }

        public DateTime RegisteredDate { get; set; } 


    }
}