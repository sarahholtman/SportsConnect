using Microsoft.EntityFrameworkCore;
using SportsConnect.Models;

namespace SportsConnect.Data
{
    public class SportsConnectContext : DbContext
    {
        public SportsConnectContext(DbContextOptions<SportsConnectContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Membership> Memberships { get; set; }
    }
}