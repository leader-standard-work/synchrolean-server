using System;
using System.Linq;
using System.Threading.Tasks;
using SynchroLean.Persistence;
using SynchroLean.Core;
using SynchroLean.Core.Models;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace SynchroLean
{
    public class Rollover
    {
        private readonly IServiceProvider _serviceProvider;

        public Rollover( IServiceProvider serviceProvider){
            _serviceProvider = serviceProvider;
        }

        public async void RunRollover()
        {
            using(var scope = _serviceProvider.CreateScope()){

                var context = scope.ServiceProvider.GetService<SynchroLeanDbContext>();
                var _unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();

                //Determine important times
                var tomorrow = DateTime.Today + TimeSpan.FromDays(1);
                var periodToNextMidnight = tomorrow - DateTime.Now;
                var endOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);
                var endOfWeek = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
                    .AddDays((int)(DayOfWeek.Saturday) - (int)(DateTime.Now.DayOfWeek) + 1);

                //Clean up the to-do list for the night
                await _unitOfWork.todoList.CleanTodos();
                _unitOfWork.CompleteAsync().Wait();

                //Do cleanup of old tasks and log entries
                await _unitOfWork.completionLogEntryRepository.CleanupLog(DateTime.Now.Date - TimeSpan.FromDays(730.5)); //2a
                await _unitOfWork.userTaskRepository.CleanTasks();

                var cal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;

                //Add daily todos
                var tasks =
                    from task in context.UserTasks
                    where task.IsRecurring
                        && !task.IsRemoved
                        && (!(task.Frequency == Frequency.Daily) || task.OccursOnDayOfWeek(DateTime.Now.DayOfWeek))
                        && !context.Todos.Any(todo => todo.TaskId == task.Id)
                        && (// Check that there's not already a log entry for current month for monthly task
                            (task.Frequency == Frequency.Monthly 
                                    && !context.TaskCompletionLog.Any(log => log.TaskId == task.Id 
                                        && log.EntryTime.Month == DateTime.Now.Month))
                            // Check that there's not already a log entry for current week for weekly task
                            || (task.Frequency == Frequency.Weekly
                                && !context.TaskCompletionLog.Any(log => log.TaskId == task.Id &&
                                    log.EntryTime.AddDays(-1 * (int)cal.GetDayOfWeek(log.EntryTime)) ==
                                    DateTime.Today.AddDays(-1 * (int)cal.GetDayOfWeek(DateTime.Today))
                                    )
                                )
                            )
                    select task;

                foreach (var task in tasks)
                {
                    var expiry =
                        task.Frequency == Frequency.Monthly ? endOfMonth
                        : task.Frequency == Frequency.Weekly ? endOfWeek
                        : task.Frequency == Frequency.Daily ? tomorrow
                        : DateTime.MaxValue;
                    context.Todos.Add(Todo.FromTask(task, expiry));
                }

                context.SaveChanges();
            }
        }
    }
}