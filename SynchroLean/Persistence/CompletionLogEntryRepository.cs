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
        /// <param name="emailAddress"></param>
        /// <param name="entryTime"></param>
        /// <returns></returns>
        public void DeleteLogEntry(int taskId, string emailAddress, DateTime entryTime)
        {
            // Create log entry to search table
            var completionLogEntry = new CompletionLogEntry{ 
                TaskId = taskId,
                OwnerEmail = emailAddress,
                EntryTime = entryTime 
            };
            // Remove entry from Db and save changes
            context.TaskCompletionLog.Remove(completionLogEntry);
        }

        /// <summary>
        /// Get the completion rate for a user
        /// </summary>
        /// <param name="emailAddress">The key to identify the owner</param>
        /// <param name="start">The date to begin metrics from</param>
        /// <param name="end">The date to end metrics from</param>
        /// <returns>The proportion (between 0 and 1) of tasks completed, which can be NaN for 0/0</returns>
        public async Task<double> GetUserCompletionRate(string emailAddress, DateTime start, DateTime end)
        {
            var userTasks = await context.TaskCompletionLog
                .Where(ut => ut.OwnerEmail.Equals(emailAddress) && ut.EntryTime > start && ut.EntryTime <= end)
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
                return double.NaN;
            }
        }

        /// <summary>
        /// Get the completion rate for a team
        /// </summary>
        /// <param name="teamId">The key to identify the team</param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns>The proportion (between 0 and 1) of tasks completed, which can be NaN for 0/0</returns>
        public async Task<double> GetTeamCompletionRate(int teamId, DateTime start, DateTime end)
        {
            var teamTasks = await
            (
                from task in context.TaskCompletionLog
                where task.TeamId != null && (int)task.TeamId == teamId && task.EntryTime > start && task.EntryTime <= end
                select task.IsCompleted ? 1.0 : 0.0
            ).ToListAsync();
            // Team has tasks
            return teamTasks.Count > 0 ? teamTasks.Average() : double.NaN;
        }

        public async Task Clean()
        {
            var startOfLastYear = new DateTime(DateTime.Now.Year, 1, 1);
            var entriesToDelete = await
                (from entry in context.TaskCompletionLog
                 where entry.EntryTime < startOfLastYear
                 select entry).ToListAsync();
            foreach (var entryToDelete in entriesToDelete) context.TaskCompletionLog.Remove(entryToDelete);
        }

        public async Task<double> GetUserCompletionRateOnTeam(string emailAddress, int teamId, DateTime start, DateTime end)
        {
            var userTeamTasks = await
               (from task in context.TaskCompletionLog
                where task.OwnerEmail == emailAddress
                && task.TeamId != null
                && teamId == (int)task.TeamId
                && task.EntryTime > start
                && task.EntryTime <= end
                select task.IsCompleted ? 1.0 : 0.0).ToListAsync();
            return userTeamTasks.Count > 0 ? userTeamTasks.Average() : double.NaN;
        }

        public async Task<double> GetUserCompletionRateOnTeams(string emailAddress, IEnumerable<int> teamId, DateTime start, DateTime end)
        {
            var userTeamTasks = await
               (from task in context.TaskCompletionLog
                where task.OwnerEmail == emailAddress
                && task.TeamId != null
                && teamId.Contains((int)task.TeamId)
                && task.EntryTime > start 
                && task.EntryTime <= end
                select task.IsCompleted ? 1.0 : 0.0).ToListAsync();
            return userTeamTasks.Count > 0 ? userTeamTasks.Average() : double.NaN;
        }
    }
}