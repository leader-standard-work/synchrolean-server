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
            await context.TeamMembers.AddAsync(new TeamMember { TeamId = team.Id, MemberId = team.OwnerId });
        }

        public async Task<Boolean> TeamExists(int teamId)
        {
            return await context.Teams
                .AnyAsync(team => team.Id == teamId);
        }
    }
}
