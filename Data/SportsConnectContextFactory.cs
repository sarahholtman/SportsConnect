using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SportsConnect.Data
{
    public class SportsConnectContextFactory : IDesignTimeDbContextFactory<SportsConnectContext>
    {
        public SportsConnectContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SportsConnectContext>();
            optionsBuilder.UseSqlite("Data Source=SportsConnect.db");

            return new SportsConnectContext(optionsBuilder.Options);
        }
    }
}