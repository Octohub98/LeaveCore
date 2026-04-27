using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LeaveCore.Models
{
    public class LeaveContextFactory : IDesignTimeDbContextFactory<LeaveContext>
    {
        public LeaveContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connectionString = configuration.GetConnectionString("LeaveConnection");
            var optionsBuilder = new DbContextOptionsBuilder<LeaveContext>();
            optionsBuilder.UseNpgsql(connectionString);
            return new LeaveContext(optionsBuilder.Options);
        }
    }
}
