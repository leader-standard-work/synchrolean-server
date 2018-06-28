using Microsoft.EntityFrameworkCore;
using SynchroLean.Models;

namespace SynchroLean.Persistence
{
    public class SynchroLeanDbContext : DbContext
    {
        public DbSet<UserTask> UserTasks { get; set; }

        public SynchroLeanDbContext(DbContextOptions<SynchroLeanDbContext> options) : base(options)
        {

        }
    }
}
