using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        public string Address { get; set; }
        public string RefreshToken { get; set; }
        [Required]
        public string Role { get; set; } = "user";
        
        [ForeignKey("AdharId")]
        public int AdharId { get; set; }
        public Adhar AdharNumber { get; set; }

        public List<Account> Accounts { get; set; }

        public DateTime RegisteredDate { get; set; } 

        
    }
}