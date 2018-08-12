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
        }

        public async Task CompleteAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
