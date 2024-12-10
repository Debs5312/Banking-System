using Microsoft.EntityFrameworkCore;

namespace Persistance
{
    public class AccountDBContext : DbContext
    {
        public AccountDBContext(DbContextOptions options) : base(options)
        {
            
        }

        
    }
}