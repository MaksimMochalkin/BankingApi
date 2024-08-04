namespace RestaurantBooking.BusinesApi.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class BankingAppDbContext : IdentityDbContext
    {
        public BankingAppDbContext(DbContextOptions<BankingAppDbContext> options)
            : base(options)
        {
        }
    }
}
