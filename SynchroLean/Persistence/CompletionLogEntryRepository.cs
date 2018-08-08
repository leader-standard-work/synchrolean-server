using Microsoft.EntityFrameworkCore;
using SynchroLean.Core;
using SynchroLean.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SynchroLean.Persistence
{
    public class CompletionLogEntryRepository : ICompletionLogEntryRepository
    {
        private readonly SynchroLeanDbContext context;

        public CompletionLogEntryRepository(SynchroLeanDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Adds a task to the lot
        /// </summary>
        /// <param name="completionLogEntry"></param>
        /// <returns></returns>
        public async Task AddLogEntryAsync(CompletionLogEntry completionLogEntry)
        {
            await context.TaskCompletionLog.AddAsync(completionLogEntry);
        }

        /// <summary>
        /// Remove a task from the log and add it to 
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public void DeleteLogEntry(int taskId, int ownerId, DateTime entryTime)
        {
            // Create log entry to search table
            CompletionLogEntry completionLogEntry = new CompletionLogEntry{ 
                TaskId = taskId,
                OwnerId = ownerId,
                EntryTime = entryTime 
            };
            // Remove entry from Db and save changes
            context.TaskCompletionLog.Remove(completionLogEntry);
        }

        /// <summary>
        /// Get the completion rate for a user
        /// </summary>
        /// <param name="ownerId">The key to identify the owner</param>
        /// <param name="start">The date to begin metrics from</param>
        /// <param name="end">The date to end metrics from</param>
        /// <returns>The proportion (between 0 and 1) of tasks completed, which can be NaN for 0/0</returns>
        public async Task<Double> GetUserCompletionRate(int ownerId, DateTime start, DateTime end)
        {
            var userTasks = await context.TaskCompletionLog
                .Where(ut => ut.OwnerId.Equals(ownerId) && ut.EntryTime > start && ut.EntryTime <= end)
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
        /// <returns>The proportion (between 0 and 1) of tasks completed, which can be NaN for 0/0</returns>
        public async Task<Double> GetTeamCompletionRate(int teamId, DateTime start, DateTime end)
        {
            var teamTasks = await
            (
                from task in context.TaskCompletionLog
                where task.TeamId != null && (int)task.TeamId == teamId && task.EntryTime > start && task.EntryTime <= end
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

        public async Task CleanupLog(DateTime threshold)
        {
            var toRemove = await context.TaskCompletionLog.Where(entry => entry.EntryTime < threshold).ToListAsync();
            context.TaskCompletionLog.RemoveRange(toRemove);
        }

        public async Task<Double> GetUserCompletionRateOnTeams(int userId, DateTime start, DateTime end, IEnumerable<int> teamId)
        {
            var userTeamTasks = await
               (from task in context.TaskCompletionLog
                where task.OwnerId == userId
                && task.TeamId != null
                && teamId.Contains((int)task.TeamId)
                && task.EntryTime > start && task.EntryTime <= end
                select task.IsCompleted ? 1.0 : 0.0).ToListAsync();
            if (userTeamTasks.Count > 0) return userTeamTasks.Average();
            else return Double.NaN;
    }
}