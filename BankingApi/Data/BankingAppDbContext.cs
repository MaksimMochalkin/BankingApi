namespace RestaurantBooking.BusinesApi.Data
{
    using BankingApi.Data.Entyties;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class BankingAppDbContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Account> Accounts { get; set; }

        public BankingAppDbContext(DbContextOptions<BankingAppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>()
            .Property(a => a.Balance)
            .HasColumnType("decimal(18, 2)");
        }
    }
}
