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
        /// <param name="completionLogEntry"></param>
        /// <param name="isCompleted"></param>
        /// <returns></returns>
        Task AddLogEntryAsync(CompletionLogEntry completionLogEntry, bool isCompleted);

        /// <summary>
        /// Delete a log entry
        /// </summary>
        /// <returns></returns>
        Task DeleteLogEntryAsync(int taskId, int ownerId, DateTime entryTime);

        /// <summary>
        /// Returns a Users completion rate
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        Task<Double> GetUserCompletionRate(int ownerId, DateTime start, DateTime end);
        
        /// <summary>
        /// Returns a Teams completion rate
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Task<Double> GetTeamCompletionRate(int teamId, DateTime start, DateTime end);
    }
}