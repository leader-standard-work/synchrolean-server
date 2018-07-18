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

        async Task ITeamMemberRepository.AddUserToTeam(int teamId, int userId)
        {
            await context.TeamMembers.AddAsync(new TeamMember { TeamId = teamId, MemberId = userId });
        }

        async Task<IEnumerable<TeamMember>> ITeamMemberRepository.GetAllTeamMemberships()
        {
            return await context.TeamMembers.ToListAsync();
        }

        async Task<IEnumerable<Team>> ITeamMemberRepository.GetAllTeamsForUser(int userId)
        {
            return await
                (
                    from teammember in context.TeamMembers
                    where teammember.MemberId == userId
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

        async Task ITeamMemberRepository.RemoveUserFromTeam(int teamId, int userId)
        {
            var team = await context.Teams.FindAsync(teamId);
            if (team == null) return; //No such team, nothing to do
            if (team.OwnerId == userId) return; //The owner of a team is always part of that team
            context.Remove(new TeamMember { TeamId = teamId, MemberId = userId });
        }

        async Task<bool> ITeamMemberRepository.UserIsInTeam(int teamId, int userId)
        {
            return await
                (
                    from teammember in context.TeamMembers
                    where teammember.TeamId == teamId
                    select teammember.Member.OwnerId
                ).ContainsAsync(userId);
        }

        async Task ITeamMemberRepository.ChangeTeamOwnership(int teamId, int newOwnerId)
        {
            //Can't make you the owner if you're not in the team.
            var userIsInTeam = await ((ITeamMemberRepository)this).UserIsInTeam(teamId, newOwnerId);
            var team = await context.Teams.FindAsync(teamId);
            if (!userIsInTeam)
            {
                //But if we're in this situation, do a consistency check
                //This is so that you can't get stuck in a bad state where a user isn't part of a team
                //but is the owner of that team.
                if(team.OwnerId == newOwnerId)
                {
                    //Fix the inconsistency
                    await ((ITeamMemberRepository)this).AddUserToTeam(teamId, newOwnerId);
                }
                //Nothing to do
                return;
            }
            //Change the team owner
            team.OwnerId = newOwnerId;
        }
    }
}
