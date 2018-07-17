using SynchroLean.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SynchroLean.Core
{
    public interface IUserTeamRepository
    {
        Task<Team> GetUserTeamAsync(int teamId);
        Task<IEnumerable<Team>> GetAllTeamsAsync();
        Task AddAsync(Team team);
        Task<Boolean> TeamExists(int teamId);
    }
}
