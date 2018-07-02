using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SynchroLean
{
    interface IUserTask
    {
        /// <summary>
        /// Which days of the week does this task happen of, if it is weekly?
        /// </summary>
        IEnumerable<DayOfWeek> Weekdays { get; set; }

        /// <summary>
        /// Tests if the event occurs on a particular weekday
        /// </summary>
        /// <param name="day">Which day of the week to test.</param>
        /// <returns>True if this event occurs on the given day.</returns>
        bool occursOnDayOfWeek(DayOfWeek day);

        /// <summary>
        /// Completes the task.
        /// </summary>
        void completeTask();

        /// <summary>
        /// Checks if this task has been completed.
        /// </summary>
        /// <returns>True if this task has been completed.</returns>
        bool IsCompleted { get; }

        /// <summary>
        /// Deletes the task.
        /// </summary>
        void deleteTask();

        /// <summary>
        /// Checks if this task is deleted.
        /// </summary>
        /// <returns>True if the task is deleted.</returns>
        bool IsDeleted { get; }


    }
}
