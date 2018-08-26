using Microsoft.EntityFrameworkCore;
using SynchroLean.Core;
using SynchroLean.Core.Models;
using SynchroLean.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SynchroLean.Persistence
{
    public class UserAccountRepository : IUserAccountRepository
    {
        private readonly SynchroLeanDbContext context;

        public UserAccountRepository(SynchroLeanDbContext context)
        {
            this.context = context;
        }

        public async Task<UserAccount> GetUserAccountAsync(string emailAddress)
        {
            return await context.UserAccounts
                .FindAsync(emailAddress.Trim().ToLower());
        }

        public async Task AddAsync(UserAccount account)
        {
            await context.UserAccounts.AddAsync(account);
        }

        public async Task<Boolean> UserAccountExists(string emailAddress)
        {
            return await context.UserAccounts
                .AnyAsync(user => user.Email == emailAddress);
        }

        public async Task Clean()
        {
            var startOfLastYear = new DateTime(DateTime.Now.Year-1, 1, 1);
            var accountsToDelete = await
                (from account in context.UserAccounts
                 where account.IsDeleted && account.Deleted < startOfLastYear
                 select account).ToListAsync();
            foreach (var accountToDelete in accountsToDelete) context.UserAccounts.Remove(accountToDelete);
        }

        public async Task DeleteAccount(string email)
        {
            var account = 
                await context.UserAccounts
                .Include(acc => acc.Tasks)
                .Include(acc => acc.TeamMembershipRelations)
                .FirstOrDefaultAsync(x => x.Email == email);
            account.Delete();
            foreach (var task in account.Tasks) task.Delete();
            foreach (var teamMembership in account.TeamMembershipRelations)
            {
                var team = await context.Teams.FindAsync(teamMembership.TeamId);
                if (team == null) continue; //No such team, nothing to do
                var othermembers =
                        from teammember in context.TeamMembers
                        where teammember.TeamId == teamMembership.TeamId && teammember.MemberEmail != email
                        select teammember.Member.Email;
                if (othermembers.Count() > 0)
                {
                    if (team.OwnerEmail == email) team.OwnerEmail = new System.Random().SampleFrom(othermembers);
                    context.Remove(new TeamMember { TeamId = teamMembership.TeamId, MemberEmail = email });
                }
                else
                {
                    context.Remove(new TeamMember { TeamId = teamMembership.TeamId, MemberEmail = email });
                    team.Delete();
                }
            }
        }
    }
}
