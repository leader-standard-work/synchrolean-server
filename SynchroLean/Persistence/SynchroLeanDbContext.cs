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
            //Composite Keys
            modelBuilder.Entity<TeamPermission>().HasKey(perm => new { perm.SubjectTeamId, perm.ObjectTeamId });
            modelBuilder.Entity<TeamMember>().HasKey(member => new { member.TeamId, MemberId = member.MemberEmail });
            modelBuilder.Entity<CompletionLogEntry>().HasKey(log => new { log.TaskId, OwnerId = log.OwnerEmail, log.EntryTime });
            modelBuilder.Entity<Todo>().HasKey(todo => todo.TaskId);
            modelBuilder.Entity<AddUserRequest>().HasKey(invite => new { invite.InviteeEmail, invite.DestinationTeamId });

            //Optional relationships
            modelBuilder.Entity<Todo>()
                .HasOne(todo => todo.Task)
                .WithOne(task => task.Todo)
                .HasForeignKey<Todo>(todo => todo.TaskId)
                .HasPrincipalKey<UserTask>(task => task.Id)
                .HasConstraintName("FK_Todo_Tasks_TaskId"); //EF Core won't generate the constraint without this, but why?

            //One-to-many relationships
            modelBuilder.Entity<Team>().HasMany(team => team.AssociatedTasks).WithOne(task => task.Team).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Team>().HasMany(team => team.AssociatedLogEntries).WithOne(entry => entry.Team).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserAccount>().HasMany(account => account.Tasks).WithOne(task => task.Owner);
            modelBuilder.Entity<Team>().HasMany(team => team.Invites).WithOne(invite => invite.DestinationTeam);
            modelBuilder.Entity<UserAccount>().HasMany(account => account.OutgoingInvites).WithOne(invite => invite.Inviter).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<UserAccount>().HasMany(account => account.IncomingInvites).WithOne(invite => invite.Invitee);

            //Many to Many relationships
            //In EF core we must explicitly name and create the joining table
            modelBuilder.Entity<TeamMember>().HasOne(membership => membership.Member).WithMany(account => account.TeamMembershipRelations);
            modelBuilder.Entity<TeamMember>().HasOne(membership => membership.Team).WithMany(team => team.TeamMembershipRelations);
            modelBuilder.Entity<TeamPermission>().HasOne(perm => perm.SubjectTeam).WithMany(team => team.PermissionsWhereThisIsSubject);
            modelBuilder.Entity<TeamPermission>().HasOne(perm => perm.ObjectTeam).WithMany(team => team.PermissionsWhereThisIsObject);
        }
    }
}
