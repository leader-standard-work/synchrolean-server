using System;
using System.Threading.Tasks;
using SynchroLean.Core.Models;

namespace SynchroLean.Core
{
    public interface ICompletionLogEntryRepository
    {
        
        /// <summary>
        /// Add a log entry
        /// </summary>
        /// <param name="completionLogEntry">Log entry to be add to Db asynchronously</param>
        /// <returns></returns>
        Task AddLogEntryAsync(CompletionLogEntry completionLogEntry);

        /// <summary>
        /// Delete a log entry
        /// </summary>
        /// <param name="taskId">Id of task</param>
        /// <param name="ownerId">Owner of task</param>
        /// <param name="entryTime">Time log entry was processed</param>
        /// <returns></returns>
        void DeleteLogEntry(int taskId, int ownerId, DateTime entryTime);

        /// <summary>
        /// Returns a Users completion rate
        /// </summary>
        /// <param name="ownerId">Owner id</param>
        /// <param name="start">Starting date for owner metrics calculations</param>
        /// <param name="end">Ending date for owner metrics calculations</param>
        /// <returns></returns>
        Task<Double> GetUserCompletionRate(int ownerId, DateTime start, DateTime end);
        
        /// <summary>
        /// Returns a Teams completion rate
        /// </summary>
        /// <param name="teamId">Team id</param>
        /// <param name="start">Starting date for team metrics calculations</param>
        /// <param name="end">Ending date for team metrics calculations</param>
        /// <returns></returns>
        Task<Double> GetTeamCompletionRate(int teamId, DateTime start, DateTime end);

        /// <summary>
        /// Deletes all log entries older than the threshold
        /// </summary>
        /// <param name="threshold">The oldest a kept log entry can be</param>
        /// <returns></returns>
        Task CleanupLog(DateTime threshold);
    }
}