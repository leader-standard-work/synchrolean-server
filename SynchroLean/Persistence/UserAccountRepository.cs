using Microsoft.EntityFrameworkCore;
using SynchroLean.Core;
using SynchroLean.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SynchroLean.Persistence
{
    public class UserAccountRepository : IUserAccountRepository
    {
        private readonly SynchroLeanDbContext context;

        public UserAccountRepository(SynchroLeanDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Retrieves a UserAccount from the database
        /// </summary>
        /// <param name="ownerId">The key of the UserAccount</param>
        /// <returns>The UserAccount fetched from the database</returns>
        public async Task<UserAccount> GetUserAccountAsync(int ownerId)
        {
            return await context.UserAccounts
                .SingleOrDefaultAsync(ua => ua.OwnerId.Equals(ownerId));
        }

        /// <summary>
        /// Retrieves a teams UserAccounts from the database
        /// </summary>
        /// <param name="teamId">The key to identify the team</param>
        /// <returns>A list of UserAccounts for the given team id</returns>
        public async Task<IEnumerable<UserAccount>> GetTeamAccountsAsync(int teamId)
        {
            return await context.UserAccounts
                .Where(ua => ua.TeamId.Equals(teamId))
                .ToListAsync();
        }

        /// <summary>
        /// Adds a UserAccount to the database
        /// </summary>
        /// <param name="account">Account to be added</param>
        /// <returns></returns>
        public async Task AddAsync(UserAccount account)
        {
            await context.UserAccounts.AddAsync(account);
        }

        /// <summary>
        /// Checks to see if an account exists in the database
        /// </summary>
        /// <param name="ownerId">Account id to check existence for</param>
        /// <returns>Boolean</returns>
        public async Task<Boolean> UserAccountExists(int ownerId)
        {
            return await context.UserAccounts
                .AnyAsync(user => user.OwnerId == ownerId);
        }
    }
}
