using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SynchroLean.Core
{
    public interface IUnitOfWork
    {
        IUserAccountRepository userAccountRepository { get; }
        IUserTeamRepository userTeamRepository { get; }
        IUserTaskRepository userTaskRepository { get; }
        IAddUserRequestRepository addUserRequestRepository { get; }
        ITeamPermissionRepository teamPermissionRepository { get; }
        ITeamMemberRepository teamMemberRepository { get; }
        ICompletionLogEntryRepository completionLogEntryRepository { get; }
        ITodoRepository todoList { get; }

        /// <summary>
        /// Saves changes to Db context asynchronously
        /// </summary>
        /// <returns></returns>
        Task CompleteAsync();
    }
}
