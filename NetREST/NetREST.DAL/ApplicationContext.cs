using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetREST.Domain.User;

namespace NetREST.DAL
{
    public class ApplicationContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options,
            ILogger<ApplicationContext> logger)
            : base(options)
        {
            Database.EnsureCreated();
            // try
            // {
            //     Database.CanConnect();
            //     logger.LogInformation("Successfully connected to database");
            // }
            // catch (SqlException ex)
            // {
            //     logger.LogError($"Database connection error: {ex}");
            // }
        }
    }
}