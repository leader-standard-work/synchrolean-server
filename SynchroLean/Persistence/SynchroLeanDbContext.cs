using Microsoft.EntityFrameworkCore;
using SynchroLean.Core.Models;

namespace SynchroLean.Persistence
{
    public class SynchroLeanDbContext : DbContext
    {
        public DbSet<UserTask> UserTasks { get; set; }
        
        public DbSet<Team> Teams { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }

        public SynchroLeanDbContext(DbContextOptions<SynchroLeanDbContext> options) : base(options)
        {

        }
    }
}
