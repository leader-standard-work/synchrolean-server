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
        /// Adds a UserTask to the database
        /// </summary>
        /// <param name="userTask">UserTask to be added</param>
        /// <returns></returns>
        Task AddAsync(UserTask task);

        /// <summary>
        /// Deletes all tasks that can be safely removed.
        /// </summary>
        /// <returns></returns>
        Task CleanTasks();
    }
}
