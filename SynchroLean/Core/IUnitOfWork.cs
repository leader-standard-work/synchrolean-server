using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SynchroLean.Core
{
    public interface IUnitOfWork
    {
        IUserAccountRepository UserAccountRepository { get; }
        IUserTeamRepository UserTeamRepository { get; }
        IUserTaskRepository UserTaskRepository { get; }
        IAddUserRequestRepository AddUserRequestRepository { get; }
        ITeamPermissionRepository TeamPermissionRepository { get; }
        ITeamMemberRepository TeamMemberRepository { get; }
        ICompletionLogEntryRepository CompletionLogEntryRepository { get; }
        ITodoRepository TodoRepository { get; }

        /// <summary>
        /// Saves changes to Db context asynchronously
        /// </summary>
        /// <returns></returns>
        Task CompleteAsync();
    }
}
