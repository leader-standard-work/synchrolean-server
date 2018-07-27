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
        public IUserTaskRepository userTaskRepository { get; }
        public IUserAccountRepository userAccountRepository { get; }
        public IUserTeamRepository userTeamRepository { get; }
        public IAddUserRequestRepository addUserRequestRepository { get; }
        public ITeamPermissionRepository teamPermissionRepository { get; }
        public ITeamMemberRepository teamMemberRepository { get; }
        public ICompletionLogEntryRepository completionLogEntryRepository { get; }
        public ITodoRepository todoList { get; }
        public UnitOfWork(SynchroLeanDbContext context)
        {
            this.context = context;
            userTaskRepository = new UserTaskRepository(context);
            userAccountRepository = new UserAccountRepository(context);
            userTeamRepository = new UserTeamRepository(context);
            addUserRequestRepository = new AddUserRequestRespository(context);
            teamPermissionRepository = new TeamPermissionRepository(context);
            teamMemberRepository = new TeamMemberRepository(context);
            completionLogEntryRepository = new CompletionLogEntryRepository(context);
            todoList = new TodoRepository(context);
            //Set up nightly rollover
            rolloverTimer = new Timer();
            rolloverTimer.Elapsed += this.handleRollover;
            rollover(true);
        }

        private void handleRollover(object sender, ElapsedEventArgs e)
        {
            rollover(false);
        }

        private void rollover(bool firstTime)
        {
            rolloverTimer.Stop();

            //Determine important times
            var tomorrow = DateTime.Today + TimeSpan.FromDays(1);
            var periodToNextMidnight = tomorrow - DateTime.Now;

            //Determine what has to be done
            var monthly = firstTime || DateTime.Now.Day == 1;
            var weekly = firstTime || DateTime.Now.DayOfWeek == DayOfWeek.Sunday;

            //Do cleanup of old tasks and log entries
            completionLogEntryRepository.CleanupLog(DateTime.Now.Date-TimeSpan.FromDays(730.5)); //2a
            // TODO: ... clean tasks here

            //Clean up the to-do list for the night
            todoList.CleanTodos(DateTime.Now.Date);

            //Add daily todos
            var tasks =
                from task in context.UserTasks
                where task.IsRecurring
                      && !task.IsRemoved
                      && task.OccursOnDayOfWeek(DateTime.Now.DayOfWeek)
                select task;

            foreach(var task in tasks)
            {
                //TODO: Use frequency when adding tasks
                context.Todos.Add(Todo.FromTask(task, tomorrow));
            }

            rolloverTimer.Interval = periodToNextMidnight.Milliseconds;
            rolloverTimer.Start();
        }

        public async Task CompleteAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
