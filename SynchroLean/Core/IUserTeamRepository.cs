using SynchroLean.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SynchroLean.Core
{
    public interface IUserTeamRepository
    {
        /// <summary>
        /// Retrieves a Team from the database
        /// </summary>
        /// <param name="teamId">The key of the Team</param>
        /// <returns>The Team fetched from the database</returns>
        Task<Team> GetUserTeamAsync(int teamId);

        /// <summary>
        /// Retrieves all teams from the database
        /// </summary>
        /// <returns>List of teams from the database</returns>
        Task<IEnumerable<Team>> GetAllTeamsAsync();
        
        /// <summary>
        /// Adds team to the database
        /// </summary>
        /// <param name="team">Team to add to the database.</param>
        /// <returns></returns>
        Task AddAsync(Team team);
        
        /// <summary>
        /// Checks for the existence of a team matching team id
        /// </summary>
        /// <param name="teamId">Team id to check existence for.</param>
        /// <returns>True if team exists, false otherwise</returns>
        Task<Boolean> TeamExists(int teamId);

        /// <summary>
        /// Mark a team as deleted and clear its tasks and users.
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        Task DeleteTeamAsync(int teamId);
    }
}
