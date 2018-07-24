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
        /// <summary>
        /// Retrieves a UserAccount with the specified email address from the database
        /// </summary>
        /// <param name="emailAddress">The email address of the UserAccount</param>
        /// <returns>The UserAccount fetched from the database</returns>
        Task<UserAccount> GetUserAccountByEmailAsync(string emailAddress);
        Task AddAsync(UserAccount account);
        Task<Boolean> UserAccountExists(int ownerId);
    }
}
