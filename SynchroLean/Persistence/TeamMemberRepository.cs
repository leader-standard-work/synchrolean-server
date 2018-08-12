using Microsoft.EntityFrameworkCore;
using SynchroLean.Core;
using SynchroLean.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SynchroLean.Persistence
{
    public class TeamMemberRepository : ITeamMemberRepository
    {
        protected SynchroLeanDbContext context;
        public TeamMemberRepository(SynchroLeanDbContext context)
        {
            this.context = context;
        }

        async Task ITeamMemberRepository.AddUserToTeam(int teamId, string userEmail)
        {
            await context.TeamMembers.AddAsync(new TeamMember { TeamId = teamId, MemberEmail = userEmail });
        }

        async Task<IEnumerable<TeamMember>> ITeamMemberRepository.GetAllTeamMemberships()
        {
            return await context.TeamMembers.ToListAsync();
        }

        async Task<IEnumerable<Team>> ITeamMemberRepository.GetAllTeamsForUser(string userEmail)
        {
            return await
                (
                    from teammember in context.TeamMembers
                    where teammember.MemberEmail == userEmail
                    select teammember.Team
                ).ToListAsync();
        }

        async Task<IEnumerable<UserAccount>> ITeamMemberRepository.GetAllUsersForTeam(int teamId)
        {
            return await
                (
                    from teammember in context.TeamMembers
                    where teammember.TeamId == teamId
                    select teammember.Member
                ).ToListAsync();
        }

        async Task ITeamMemberRepository.RemoveUserFromTeam(int teamId, string userEmail)
        {
            var team = await context.Teams.FindAsync(teamId);
            if (team == null) return; //No such team, nothing to do
            if (team.OwnerEmail == userEmail) return; //The owner of a team is always part of that team
            context.Remove(new TeamMember { TeamId = teamId, MemberEmail = userEmail });
        }

        async Task<bool> ITeamMemberRepository.UserIsInTeam(int teamId, string userEmail)
        {
            return await
                (
                    from teammember in context.TeamMembers
                    where teammember.TeamId == teamId
                    select teammember.Member.Email
                ).ContainsAsync(userEmail);
        }

        async Task ITeamMemberRepository.ChangeTeamOwnership(int teamId, string newOwnerEmail)
        {
            //Can't make you the owner if you're not in the team.
            var userIsInTeam = await ((ITeamMemberRepository)this).UserIsInTeam(teamId, newOwnerEmail);
            var team = await context.Teams.FindAsync(teamId);
            if (!userIsInTeam)
            {
                //But if we're in this situation, do a consistency check
                //This is so that you can't get stuck in a bad state where a user isn't part of a team
                //but is the owner of that team.
                if(team.OwnerEmail == newOwnerEmail)
                {
                    //Fix the inconsistency
                    await ((ITeamMemberRepository)this).AddUserToTeam(teamId, newOwnerEmail);
                }
                //Nothing to do
                return;
            }
            //Change the team owner
            team.OwnerEmail = newOwnerEmail;
        }
    }
}
