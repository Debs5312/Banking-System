using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class RegistrationDTO
    {
        public string UserName { get; set; }
        public string HashedPassword { get; set; }
        public string Address { get; set; }
        public int AdharNumber { get; set; }
    }
}