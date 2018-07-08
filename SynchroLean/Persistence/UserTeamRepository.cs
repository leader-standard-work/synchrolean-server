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

        /// <summary>
        /// Retrieves a Team from the database
        /// </summary>
        /// <param name="teamId">The key of the Team</param>
        /// <returns>The Team fetched from the database</returns>
        public async Task<Team> GetUserTeamAsync(int teamId)
        {
            return await context.Teams
                .SingleOrDefaultAsync(ut => ut.OwnerId.Equals(teamId));
        }

        /// <summary>
        /// Retrieves all teams from the database
        /// </summary>
        /// <returns>List of teams from the database</returns>
        public async Task<IEnumerable<Team>> GetAllTeamsAsync()
        {
            return await context.Teams.ToListAsync();
        }

        /// <summary>
        /// Adds team to the database
        /// </summary>
        /// <param name="team">Team to add to the database.</param>
        /// <returns></returns>
        public async Task AddAsync(Team team)
        {
            await context.Teams.AddAsync(team);
        }

        /// <summary>
        /// Checks for the existence of a team matching team id
        /// </summary>
        /// <param name="teamId">Team id to check existence for.</param>
        /// <returns>Boolean</returns>
        public async Task<Boolean> TeamExists(int teamId)
        {
            return await context.Teams
                .AnyAsync(team => team.Id == teamId);
        }
    }
}
