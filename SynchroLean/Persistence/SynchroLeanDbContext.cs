using Microsoft.EntityFrameworkCore;
using SynchroLean.Core.Models;

namespace SynchroLean.Persistence
{
    public class SynchroLeanDbContext : DbContext
    {
        public DbSet<UserTask> UserTasks { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<AddUserRequest> AddUserRequests { get; set; }
        public DbSet<TeamPermission> TeamPermissions { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<CompletionLogEntry> TaskCompletionLog { get; set; }
        public DbSet<Todo> Todos { get; set; }
        public SynchroLeanDbContext(DbContextOptions<SynchroLeanDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TeamPermission>().HasKey(perm => new { perm.SubjectTeamId, perm.ObjectTeamId });
            modelBuilder.Entity<TeamMember>().HasKey(member => new { member.TeamId, member.MemberId });
            modelBuilder.Entity<CompletionLogEntry>().HasKey(log => new { log.TaskId, log.OwnerId, log.EntryTime });
        }
    }
}
