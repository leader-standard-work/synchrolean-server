using SynchroLean.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SynchroLean.Core
{
    public interface IUserTaskRepository
    {
        Task<IEnumerable<UserTask>> GetTasksAsync(int ownerId);
        Task<UserTask> GetTaskAsync(int taskId);
        Task<Double> GetUserCompletionRate(int ownerId);
        Task<Double> GetTeamCompletionRate(int ownerId);
        Task AddAsync(UserTask task);
    }
}
