using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SynchroLean.Controllers.Resources;
using SynchroLean.Models;
using SynchroLean.Persistence;

namespace SynchroLean.Controllers
{
    /// <summary>
    /// This class handles HTTP requests for accounts
    /// </summary>
    [Route("api/[controller]")]
    public class AccountsController : Controller
    {
        private readonly SynchroLeanDbContext context;

        public AccountsController(SynchroLeanDbContext context)
        {
            this.context = context;    
        }

        // Post api/accounts
        /// <summary>
        /// Adds new account to UserAccounts table in Db
        /// </summary>
        /// <param name="userAccountResource"></param>
        /// <returns>
        /// New account retrieved from Db
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> AddUserTaskAsync([FromBody]UserAccountResource userAccountResource)
        {
            // How does this validate against the UserAccount model?
            if(!ModelState.IsValid) {
                return BadRequest();
            }

            // Map account resource to model
            var account = new UserAccount 
            {
                OwnerId = userAccountResource.OwnerId,
                TeamId = userAccountResource.TeamId,
                FirstName = userAccountResource.FirstName,
                LastName = userAccountResource.LastName,
                Email = userAccountResource.Email,
                IsDeleted = userAccountResource.IsDeleted
            };

            // Add model to database and save changes
            await context.AddAsync(account);
            await context.SaveChangesAsync();

            // Retrieve account from database
            var accountModel = await context.UserAccounts
                .SingleOrDefaultAsync(ua => ua.OwnerId.Equals(account.OwnerId));

            // Map account model to resource
            var outResource = new UserAccountResource
            {
                OwnerId = accountModel.OwnerId,
                TeamId = accountModel.TeamId,
                FirstName = accountModel.FirstName,
                LastName = accountModel.LastName,
                Email = accountModel.Email,
                IsDeleted = accountModel.IsDeleted
            };
            // Rerturn account resource
            return Ok(outResource);
        }

        // GET api/accounts
        /// <summary>
        /// Retrieves accounts from UserAccount table in Db
        /// </summary>
        /// <returns>
        /// List of accounts from UserAccount
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetAccountAsync()
        {
            // Fetch all account from the DB asyncronously
            var accounts = await context.UserAccounts.ToListAsync<UserAccount>();

            // List of corresponding accounts as resources
            var resourceAccounts = new List<UserAccountResource>();

            // Retrive accounts from database
            accounts.ForEach(account =>
            {
                // Map account model to resource
                var resourceAccount = new UserAccountResource 
                {
                    OwnerId = account.OwnerId,
                    TeamId = account.TeamId,
                    FirstName = account.FirstName,
                    LastName = account.LastName,
                    Email = account.Email,
                    IsDeleted = account.IsDeleted
                };
                // Add resource to account list
                resourceAccounts.Add(resourceAccount);
            });

            // Return account resource
            return Ok(resourceAccounts);
        }

        // PUT api/accounts/{ownerId}
        [HttpPut("{ownerId}")]
        public async Task<IActionResult> EditAccountAsync(int ownerId, [FromBody]UserAccountResource userAccountResource)
        {
            // How does this validate against the UserAccount model?
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            // Retrieve ownerId account from database
            var account = await context.UserAccounts
                .SingleOrDefaultAsync(ua => ua.OwnerId == ownerId);

            // No account matches ownerId
            if(account == null)
            {
                return NotFound();
            }

            // Map account resource to model
            account.TeamId = userAccountResource.TeamId;
            account.FirstName = userAccountResource.FirstName;
            account.LastName = userAccountResource.LastName;
            account.Email = userAccountResource.Email;
            account.IsDeleted = userAccountResource.IsDeleted;

            // Save updated account to database
            await context.SaveChangesAsync();

            // Map account model to resource
            var outResource = new UserAccountResource
            {
                OwnerId = account.OwnerId,
                TeamId = account.TeamId,
                FirstName = account.FirstName,
                LastName = account.LastName,
                Email = account.Email,
                IsDeleted = account.IsDeleted
            };

            // Return resource
            return Ok(outResource);
        }
    }
}