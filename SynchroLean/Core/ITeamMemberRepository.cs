using SynchroLean.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SynchroLean.Core
{
    public interface ITeamMemberRepository
    {
        /// <summary>
        /// Add a user into a team.
        /// </summary>
        /// <param name="teamId">The team the user will be added into.</param>
        /// <param name="userEmail">The user that will be added to the team.</param>
        /// <returns></returns>
        Task AddUserToTeam(int teamId, string userEmail);

        /// <summary>
        /// Remove a user from a team.
        /// </summary>
        /// <param name="teamId">The team the user will be removed from.</param>
        /// <param name="userEmail">The user that will be removed from the team.</param>
        /// <returns></returns>
        Task RemoveUserFromTeam(int teamId, string userEmail);

        /// <summary>
        /// Get all the membership relationships from the database.
        /// </summary>
        /// <returns>All team membership relationships.</returns>
        Task<IEnumerable<TeamMember>> GetAllTeamMemberships();

        /// <summary>
        /// Get all the teams a user belongs to.
        /// </summary>
        /// <param name="userEmail">The user in the teams.</param>
        /// <returns>All teams that the user belongs to.</returns>
        Task<IEnumerable<Team>> GetAllTeamsForUser(string userEmail);

        /// <summary>
        /// Get all the users in a team.
        /// </summary>
        /// <param name="teamId">The team that the users are a part of.</param>
        /// <returns>A list of all users in the team.</returns>
        Task<IEnumerable<UserAccount>> GetAllUsersForTeam(int teamId);

        /// <summary>
        /// Test if a user is in a team.
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        Task<bool> UserIsInTeam(int teamId, string userEmail);

        /// <summary>
        /// Transfer ownership of a team to a new team member.
        /// </summary>
        /// <param name="teamId">The team whose ownership will be changed.</param>
        /// <param name="newOwnerEmail">The new owner of the team.</param>
        /// <returns></returns>
        Task ChangeTeamOwnership(int teamId, string newOwnerEmail);
    }
}
