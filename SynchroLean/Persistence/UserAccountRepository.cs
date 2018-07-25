using Microsoft.EntityFrameworkCore;
using SynchroLean.Core;
using SynchroLean.Core.Models;
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

        public async Task<UserAccount> GetUserAccountAsync(int ownerId)
        {
            return await context.UserAccounts
                .SingleOrDefaultAsync(ua => ua.OwnerId.Equals(ownerId));
        }

        public async Task<UserAccount> GetUserAccountByEmailAsync(string emailAddress)
        {
            return await context.UserAccounts
                .SingleOrDefaultAsync(ua => ua.Email.Trim().ToLower().Equals(emailAddress.Trim().ToLower()));
        }

        public async Task AddAsync(UserAccount account)
        {
            await context.UserAccounts.AddAsync(account);
        }

        public async Task<Boolean> UserAccountExists(int ownerId)
        {
            return await context.UserAccounts
                .AnyAsync(user => user.OwnerId == ownerId);
        }
    }
}
