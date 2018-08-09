using Microsoft.EntityFrameworkCore;
using SynchroLean.Core;
using SynchroLean.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SynchroLean.Persistence
{
    public class UserTaskRepository : IUserTaskRepository
    {
        private readonly SynchroLeanDbContext context;

        public UserTaskRepository(SynchroLeanDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<UserTask>> GetTasksAsync(int ownerId)
        {
            return await context.UserTasks
                .Where(ut => ut.OwnerId.Equals(ownerId))
                .ToListAsync();
        }

        public async Task<UserTask> GetTaskAsync(int taskId)
        {
            return await context.UserTasks.FindAsync(taskId);
        }

        public async Task AddAsync(UserTask userTask)
        {
            await context.UserTasks.AddAsync(userTask);
        }

        public async Task<IEnumerable<UserTask>> GetTeamTasksAsync(int teamId)
        {
            return await context.UserTasks.Where(task => task.TeamId != null && (int)task.TeamId == teamId).ToListAsync();
        }

        public async Task CleanTasks()
        {
            //Left outer join
            var tasksToDelete = await
                (from task in context.UserTasks
                 join entry in context.TaskCompletionLog on task.Id equals entry.TaskId into taskentries
                 from taskentry in taskentries.DefaultIfEmpty()
                 where task.IsRemoved && taskentry == null
                 select task).ToListAsync();
            foreach (var taskToDelete in tasksToDelete) context.UserTasks.Remove(taskToDelete);
        }
    }
}
