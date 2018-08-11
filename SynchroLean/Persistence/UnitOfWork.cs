using SynchroLean.Core;
using System;
using System.Collections.Generic;
using SynchroLean.Core.Models;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;

namespace SynchroLean.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        /** 
         * We include the context in UoW and also include each of our
         * different repositories in here as well. By doing this we can
         * ensure that each repository is instantiated with the same 
         * context. This also means that each controller only needs the
         * UoW class in order to have access to the repositories rather 
         * than exposing the repositories/context in the controller.
         **/
        private readonly SynchroLeanDbContext context;
        public Timer rolloverTimer;
        public IUserTaskRepository UserTaskRepository { get; }
        public IUserAccountRepository UserAccountRepository { get; }
        public IUserTeamRepository UserTeamRepository { get; }
        public IAddUserRequestRepository AddUserRequestRepository { get; }
        public ITeamPermissionRepository TeamPermissionRepository { get; }
        public ITeamMemberRepository TeamMemberRepository { get; }
        public ICompletionLogEntryRepository CompletionLogEntryRepository { get; }
        public ITodoRepository TodoRepository { get; }
        public UnitOfWork(SynchroLeanDbContext context)
        {
            this.context = context;
            UserTaskRepository = new UserTaskRepository(context);
            UserAccountRepository = new UserAccountRepository(context);
            UserTeamRepository = new UserTeamRepository(context);
            AddUserRequestRepository = new AddUserRequestRespository(context);
            TeamPermissionRepository = new TeamPermissionRepository(context);
            TeamMemberRepository = new TeamMemberRepository(context);
            CompletionLogEntryRepository = new CompletionLogEntryRepository(context);
            TodoRepository = new TodoRepository(context);
            //Set up nightly rollover
            rolloverTimer = new Timer();
            rolloverTimer.Elapsed += this.handleRollover;
            var noTodos = context.Todos.Count() == 0;
            rollover();
        }

        private void handleRollover(object sender, ElapsedEventArgs e)
        {
            rollover();
        }

        private void rollover()
        {
            rolloverTimer.Stop();

            //Determine important times
            var tomorrow = DateTime.Today + TimeSpan.FromDays(1);
            var periodToNextMidnight = tomorrow - DateTime.Now;
            var endOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);
            var endOfWeek = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
                .AddDays((int)(DayOfWeek.Saturday) - (int)(DateTime.Now.DayOfWeek) + 1);

            //Clean up the to-do list for the night
            TodoRepository.CleanTodos();
            CompleteAsync().Wait();

            //Do cleanup of old tasks and log entries
            CompletionLogEntryRepository.CleanupLog(DateTime.Now.Date-TimeSpan.FromDays(730.5)); //2a
            UserTaskRepository.CleanTasks();
            
            //Add daily todos
            var tasks = 
                from task in context.UserTasks
                where task.IsRecurring
                      && !task.IsDeleted
                      && (task.Frequency != Frequency.Daily || task.OccursOnDayOfWeek(DateTime.Now.DayOfWeek))
                      && !context.Todos.Any(todo => todo.TaskId == task.Id)
                select task;

            foreach(var task in tasks)
            {
                var expiry =
                    task.Frequency == Frequency.Monthly ? endOfMonth
                    : task.Frequency == Frequency.Weekly ? endOfWeek
                    : task.Frequency == Frequency.Daily ? tomorrow
                    : DateTime.MaxValue;
                context.Todos.Add(Todo.FromTask(task, expiry));
            }

            context.SaveChanges();

            rolloverTimer.Interval = periodToNextMidnight.Milliseconds;
            rolloverTimer.Start();
        }

        public async Task CompleteAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
