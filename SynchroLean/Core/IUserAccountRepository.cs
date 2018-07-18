using SynchroLean.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SynchroLean.Core
{
    public interface IUserAccountRepository
    {
        Task<UserAccount> GetUserAccountAsync(int ownerId);
        Task AddAsync(UserAccount account);
        Task<Boolean> UserAccountExists(int ownerId);
    }
}
