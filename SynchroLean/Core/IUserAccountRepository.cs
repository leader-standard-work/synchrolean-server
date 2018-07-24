using SynchroLean.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SynchroLean.Core
{
    public interface IUserAccountRepository
    {
        /// <summary>
        /// Retrieves a UserAccount from the database
        /// </summary>
        /// <param name="ownerId">The key of the UserAccount</param>
        /// <returns>The UserAccount fetched from the database</returns>
        Task<UserAccount> GetUserAccountAsync(int ownerId);

        /// <summary>
        /// Retrieves a UserAccount with the specified email address from the database
        /// </summary>
        /// <param name="emailAddress">The email address of the UserAccount</param>
        /// <returns>The UserAccount fetched from the database</returns>
        Task<UserAccount> GetUserAccountByEmailAsync(string emailAddress);

        /// <summary>
        /// Adds a UserAccount to the database
        /// </summary>
        /// <param name="account">Account to be added</param>
        /// <returns></returns>
        Task AddAsync(UserAccount account);

        /// <summary>
        /// /// Checks to see if an account exists in the database
        /// </summary>
        /// <param name="ownerId">Account id to check existence for</param>
        /// <returns>True if account exists, false otherwise</returns>
        Task<Boolean> UserAccountExists(int ownerId);

    }
}
