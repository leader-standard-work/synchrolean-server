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
            return await context.UserTasks
                .Where(ut => ut.Id.Equals(taskId))
                .SingleOrDefaultAsync();
        }

        public async Task AddAsync(UserTask userTask)
        {
            await context.UserTasks.AddAsync(userTask);
        }
    }
}
