using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class Adhar
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(10000000, 99999999)]
        public int Number { get; set; }

        public User AdharCardHolder { get; set; }

        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}