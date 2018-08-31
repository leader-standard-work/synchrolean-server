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

        public async Task<IEnumerable<UserTask>> GetAllTasksAsync()
        {
            return await context.UserTasks.Include(task => task.Todo).ToListAsync();
        }

        public async Task<IEnumerable<UserTask>> GetTasksAsync(string emailAddress)
        {
            return await context.UserTasks
                .Include(task => task.Todo)
                .Where(ut => ut.OwnerEmail.Equals(emailAddress))
                .ToListAsync();
        }

        public async Task<UserTask> GetTaskAsync(int taskId)
        {
            return await context.UserTasks.Include(task => task.Todo).FirstOrDefaultAsync(task => task.Id == taskId);
        }

        public async Task AddAsync(UserTask userTask)
        {
            await context.UserTasks.AddAsync(userTask);
        }

        public async Task<IEnumerable<UserTask>> GetTeamTasksAsync(int teamId)
        {
            return await context.UserTasks.Where(task => task.TeamId != null && (int)task.TeamId == teamId).ToListAsync();
        }

        public async Task Clean()
        {
            var startOfLastYear = new DateTime(DateTime.Now.Year-1, 1, 1);
            var tasksToDelete = await
                (from task in context.UserTasks
                 where task.IsDeleted && task.Deleted < startOfLastYear
                 select task).ToListAsync();
            foreach (var taskToDelete in tasksToDelete) context.UserTasks.Remove(taskToDelete);
        }

        public async Task<IEnumerable<UserTask>> GetOrphanedTasks(string userEmail)
        {
            var tasksToReturn =
                from task in context.UserTasks.Include(task => task.Team).ThenInclude(team => team.Members)
                where 
                    task.OwnerEmail == userEmail && 
                    task.Deleted == null && 
                    task.Team != null && 
                    (task.Team.IsDeleted || !(task.Team.Members.Any(member => member.Email == userEmail)))
                select task;
            return await tasksToReturn.ToListAsync();
        }
    }
}
