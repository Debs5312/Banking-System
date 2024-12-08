using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class UserReadDTO
    {
        public string UserName { get; set; }
        public string Role { get; set; }
        public string RefreshToken { get; set; }
        public List<Account> Accounts { get; set; }
    }
}