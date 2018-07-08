using Microsoft.EntityFrameworkCore;
using SynchroLean.Core;
using SynchroLean.Models;
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

        /// <summary>
        /// Retrieves a user's UserTasks from the database
        /// </summary>
        /// <param name="ownerId">The key to identify the owner</param>
        /// <returns>A list of UserTasks for the owner</returns>
        public async Task<IEnumerable<UserTask>> GetTasksAsync(int ownerId)
        {
            return await context.UserTasks
                .Where(ut => ut.OwnerId.Equals(ownerId))
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a UserTask from the database
        /// </summary>
        /// <param name="taskId">The key of the UserTask</param>
        /// <returns>The UserTask fetched from the database</returns>
        public async Task<UserTask> GetTaskAsync(int taskId)
        {
            return await context.UserTasks
                .Where(ut => ut.Id.Equals(taskId))
                .SingleOrDefaultAsync();
        }

        /// <summary>
        /// Get the completion rate for a user
        /// </summary>
        /// <param name="ownerId">The key to identify the owner</param>
        /// <returns>The proportion (between 0 and 1) of tasks completed</returns>
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

        /// <summary>
        /// Get the completion rate for a team
        /// </summary>
        /// <param name="teamId">The key to identify the team</param>
        /// <returns>The proportion (between 0 and 1) of tasks completed</returns>
        public async Task<Double> GetTeamCompletionRate(int teamId)
        {
            var teamTasks = await
            (
                from task in context.UserTasks
                join member in (from user in context.UserAccounts
                                where user.TeamId.Equals(teamId)
                                select user.OwnerId
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

        /// <summary>
        /// Adds a UserTask to the database
        /// </summary>
        /// <param name="userTask">UserTask to be added</param>
        /// <returns></returns>
        public async Task AddAsync(UserTask userTask)
        {
            await context.UserTasks.AddAsync(userTask);
        }
    }
}
