using SynchroLean.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SynchroLean.Core
{
    public interface ITeamPermissionRepository
    {
        /// <summary>
        /// Register that a team is permitted to see detailed stats of another team.
        /// </summary>
        /// <param name="subjectId">The team that is permitted to see detailed stats for the object team.</param>
        /// <param name="objectId">The team that the subject may see detailed stats for.</param>
        /// <returns></returns>
        Task Permit(int subjectId, int objectId);

        /// <summary>
        /// Register that a team is forbidden from seeing detailed stats of another team.
        /// </summary>
        /// <param name="subjectId">The team that is forbidden from seeing detailed stats for the object team.</param>
        /// <param name="objectId">The team that the subject may not see detailed stats for.</param>
        /// <returns></returns>
        Task Forbid(int subjectId, int objectId);

        /// <summary>
        /// Test if a team is permitted to see another team.
        /// </summary>
        /// <param name="subjectId">The team that is being tested for having permission to see the object team.</param>
        /// <param name="ObjectId">The team for which the subject team's permission to see is being tested for.</param>
        /// <returns>True if the subject team can see the object team.</returns>
        Task<bool> IsPermitted(int subjectId, int objectId);

        /// <summary>
        /// Get all the team permissions in the database.
        /// </summary>
        /// <returns>All the team permissions in the database.</returns>
        Task<IEnumerable<TeamPermission>> GetTeamPermissions();

        /// <summary>
        /// Get all the teams which the input team can be seen by.
        /// </summary>
        /// <param name="objectId">The id of the object team.</param>
        /// <returns>The ids of all subject teams that can see the object team.</returns>
        Task<IEnumerable<Team>> GetTeamsThatCanSee(int objectId);

        /// <summary>
        /// Get all the teams which the input team can see.
        /// </summary>
        /// <param name="subjectId">The id of the subject team.</param>
        /// <returns>The ids of all object teams that the subject team can see.</returns>
        Task<IEnumerable<Team>> GetTeamsThatItSees(int subjectId);

        /// <summary>
        /// Test if a user can see a target team
        /// </summary>
        /// <param name="subjectUserId">The user that is trying to see team info</param>
        /// <param name="objectId">The team that the user is trying to see</param>
        /// <returns></returns>
        Task<bool> UserIsPermittedToSeeTeam(int subjectUserId, int objectId);

        /// <summary>
        /// Test if a user can see a target user
        /// </summary>
        /// <param name="subjectUserId">The user that is trying to see detailed user info</param>
        /// <param name="objectUserId">The user that the user is trying to see</param>
        /// <returns></returns>
        Task<bool> UserIsPermittedToSeeUser(int subjectUserId, int objectUserId);

        /// <summary>
        /// Get all teams ids for which a user can see detailed stats
        /// </summary>
        /// <param name="userId">The user which is a subject in the permissions model</param>
        /// <returns>All the valid object teams that can be seen</returns>
        Task<IEnumerable<int>> GetTeamIdsUserIdSees(int userId);
    }
}
