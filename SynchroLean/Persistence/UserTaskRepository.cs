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

        [Obsolete]
        public async Task<Double> GetUserCompletionRate(int ownerId)
        {
            var userTasks = await context.UserTasks
                .Where(ut => ut.OwnerId.Equals(ownerId))
                .Select(ut => ut.IsCompleted ? 1.0 : 0.0)
                .ToListAsync();
            if (userTasks.Count > 0)
            {
                return userTasks.Average();
            }
            else
            {
                //NaN or 1 are the sensible values here, depending on interpretation
                //If it is a mean, the empty average is 0/0, or NaN
                //If it is a question about if the user completed all their tasks, then
                // vacuously they did because they had none.
                //Provisionally, I am using NaN, because it is distinct from 1
                return Double.NaN;
            }
        }

        public async Task<Double> GetTeamCompletionRate(int teamId)
        {
            var teamTasks = await
            (
                from task in context.UserTasks
                join member in (from member in context.TeamMembers
                                where member.TeamId == teamId
                                select member.MemberId
                               )
                on task.OwnerId equals member
                select task.IsCompleted ? 1.0 : 0.0
            ).ToListAsync();
            // Team has tasks
            if (teamTasks.Count > 0)
            {
                return teamTasks.Average();
            }
            else
            {
                return Double.NaN;
            }
        }

        public async Task AddAsync(UserTask userTask)
        {
            await context.UserTasks.AddAsync(userTask);
        }
    }
}
