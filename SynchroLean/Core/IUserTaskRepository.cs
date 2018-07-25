using SynchroLean.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SynchroLean.Core
{
    public interface IUserTaskRepository
    {
        /// <summary>
        /// Retrieves a user's UserTasks from the database
        /// </summary>
        /// <param name="ownerId">The key to identify the owner</param>
        /// <returns>A list of UserTasks for the owner</returns>
        Task<IEnumerable<UserTask>> GetTasksAsync(int ownerId);

        /// <summary>
        /// Retrieves a UserTask from the database
        /// </summary>
        /// <param name="taskId">The key of the UserTask</param>
        /// <returns>The UserTask fetched from the database</returns>
        Task<UserTask> GetTaskAsync(int taskId);

        /// <summary>
        /// Get the completion rate for a user
        /// </summary>
        /// <param name="ownerId">The key to identify the owner</param>
        /// <returns>The proportion (between 0 and 1) of tasks completed</returns>
        Task<Double> GetUserCompletionRate(int ownerId);

        /// <summary>
        /// Get the completion rate for a team
        /// </summary>
        /// <param name="teamId">The key to identify the team</param>
        /// <returns>The proportion (between 0 and 1) of tasks completed</returns>
        Task<Double> GetTeamCompletionRate(int ownerId);

        /// <summary>
        /// Adds a UserTask to the database
        /// </summary>
        /// <param name="userTask">UserTask to be added</param>
        /// <returns></returns>
        Task AddAsync(UserTask task);
    }
}
