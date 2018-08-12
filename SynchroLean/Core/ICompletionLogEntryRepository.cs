using System;
using System.Collections.Generic;
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
        /// <param name="emailAddress">Owner of task</param>
        /// <param name="entryTime">Time log entry was processed</param>
        /// <returns></returns>
        void DeleteLogEntry(int taskId, string emailAddress, DateTime entryTime);

        /// <summary>
        /// Returns a Users completion rate
        /// </summary>
        /// <param name="emailAddress">Owner id</param>
        /// <param name="start">Starting date for owner metrics calculations</param>
        /// <param name="end">Ending date for owner metrics calculations</param>
        /// <returns></returns>
        Task<double> GetUserCompletionRate(string emailAddress, DateTime start, DateTime end);
        
        /// <summary>
        /// Returns a Teams completion rate
        /// </summary>
        /// <param name="teamId">Team id</param>
        /// <param name="start">Starting date for team metrics calculations</param>
        /// <param name="end">Ending date for team metrics calculations</param>
        /// <returns></returns>
        Task<double> GetTeamCompletionRate(int teamId, DateTime start, DateTime end);

        /// <summary>
        /// Deletes all log entries older than the threshold
        /// </summary>
        /// <param name="threshold">The oldest a kept log entry can be</param>
        /// <returns></returns>
        Task CleanupLog(DateTime threshold);

        /// <summary>
        /// Get the user's completion rate for a particular team
        /// </summary>
        /// <param name="emailAddress">The user</param>
        /// <param name="teamId">The team</param>
        /// <param name="start">Starting date for metrics calculation</param>
        /// <param name="end">Ending date for metrics calculation</param>
        /// <returns>A rate from 0 to 1, NaN if the user had no tasks assigned on those teams.</returns>
        Task<double> GetUserCompletionRateOnTeam(string emailAddress, int teamId, DateTime start, DateTime end);

        /// <summary>
        /// Get the user's completion rate for multiple teams
        /// </summary>
        /// <param name="emailAddress">The user</param>
        /// <param name="teamIds">The teams the user is one</param>
        /// <param name="start">Start date for metrics calculation</param>
        /// <param name="end">End date for metrics calculations</param>
        /// <returns>A rate from 0 to 1, NaN if the user had no tasks assigned on those teams.</returns>
        Task<double> GetUserCompletionRateOnTeams(string emailAddress, IEnumerable<int> teamIds, DateTime start, DateTime end);
    }
}