using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;

namespace Persistance
{
    public class AppDBContext : DbContext
    {
        public AppDBContext()
        {
            
        }
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Connection connection = new Connection();
            optionsBuilder.UseSqlServer(connection.ConnectionString)
            .EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
            .Property(b => b.Id)
            .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<User>()
            .Property(b => b.Id)
            .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<Adhar>()
            .Property(b => b.Id)
            .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<User>().HasOne(usr => usr.PrimaryAccount)
            .WithOne(accnt => accnt.PrimaryAccountHolder)
            .HasForeignKey<Account>(key => key.PrimaryUserId);

            modelBuilder.Entity<User>().HasMany(usr => usr.SecondaryAccounts)
            .WithOne(accnt => accnt.SecondaryAccountHolder)
            .HasForeignKey(key => key.SecondaryUserId);

            modelBuilder.Entity<User>().HasMany(usr => usr.Nominees)
            .WithOne(accnt => accnt.Nominee)
            .HasForeignKey(key => key.NomineeId);

            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Adhar> Adhars { get; set; }


    }
}