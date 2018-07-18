using SynchroLean.Core;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public IUserTaskRepository userTaskRepository { get; }
        public IUserAccountRepository userAccountRepository { get; }
        public IUserTeamRepository userTeamRepository { get; }
        public IAddUserRequestRepository addUserRequestRepository { get; }
        public ITeamPermissionRepository teamPermissionRepository { get; }
        public ITeamMemberRepository teamMemberRepository { get; }
        public UnitOfWork(SynchroLeanDbContext context)
        {
            this.context = context;
            userTaskRepository = new UserTaskRepository(context);
            userAccountRepository = new UserAccountRepository(context);
            userTeamRepository = new UserTeamRepository(context);
            addUserRequestRepository = new AddUserRequestRespository(context);
            teamPermissionRepository = new TeamPermissionRepository(context);
            teamMemberRepository = new TeamMemberRepository(context);
        }

        public async Task CompleteAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
