using Microsoft.EntityFrameworkCore;
using SynchroLean.Core;
using SynchroLean.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SynchroLean.Persistence
{
    public class UserTeamRepository : IUserTeamRepository
    {
        private readonly SynchroLeanDbContext context;

        public UserTeamRepository(SynchroLeanDbContext context)
        {
            this.context = context;
        }

        public async Task<Team> GetUserTeamAsync(int teamId)
        {
            return await context.Teams
                .SingleOrDefaultAsync(ut => ut.Id.Equals(teamId));
        }

        public async Task<IEnumerable<Team>> GetAllTeamsAsync()
        {
            return await context.Teams.ToListAsync();
        }

        public async Task AddAsync(Team team)
        {
            await context.Teams.AddAsync(team);
            await context.TeamMembers.AddAsync(new TeamMember { TeamId = team.Id, MemberEmail = team.OwnerEmail });
        }

        public async Task<Boolean> TeamExists(int teamId)
        {
            return await context.Teams
                .AnyAsync(team => team.Id == teamId);
        }

        public async Task DeleteTeamAsync(int teamId)
        {
            var team = await context.Teams.FindAsync(teamId);
            var members = await context.TeamMembers.Where(member => member.TeamId == teamId).ToListAsync();
            foreach (var member in members) context.TeamMembers.Remove(member);
            team.Delete();
        }

        public async Task Clean()
        {
            var startOfLastYear = new DateTime(DateTime.Now.Year, 1, 1);
            var teamsToDelete = await
                (from team in context.Teams
                 where team.IsDeleted && team.Deleted < startOfLastYear
                 select team).ToListAsync();
            foreach (var teamToDelete in teamsToDelete) context.Teams.Remove(teamToDelete);
        }
    }
}
