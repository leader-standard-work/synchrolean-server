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
        Task CompleteAsync();
    }
}
