using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SynchroLean.Controllers.Resources;
using SynchroLean.Models;
using SynchroLean.Persistence;
using SynchroLean.Core;

namespace SynchroLean.Controllers
{
    /// <summary>
    /// This class handles HTTP requests for accounts
    /// </summary>
    [Route("api/[controller]")]
    public class AccountsController : Controller
    {
        private readonly SynchroLeanDbContext context;
        private readonly IUnitOfWork unitOfWork;

        public AccountsController(SynchroLeanDbContext context, IUnitOfWork unitOfWork)
        {
            this.context = context;
            this.unitOfWork = unitOfWork;
        }

        // Post api/accounts
        /// <summary>
        /// Adds new account to UserAccounts table in Db
        /// </summary>
        /// <param name="userAccountResource"></param>
        /// <returns>New account retrieved from Db</returns>
        [HttpPost]
        public async Task<IActionResult> AddUserAccountAsync([FromBody]UserAccountResource userAccountResource)
        {
            // How does this validate against the UserAccount model?
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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
            //await context.AddAsync(account);
            //await context.SaveChangesAsync();
            await unitOfWork.userAccountRepository.AddAsync(account);
            await unitOfWork.CompleteAsync();

            // Retrieve account from database
            //var accountModel = await context.UserAccounts
            //    .SingleOrDefaultAsync(ua => ua.OwnerId.Equals(account.OwnerId));
            var accountModel = await unitOfWork.userAccountRepository
                .GetUserAccountAsync(account.OwnerId);

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

        // GET api/accounts/owner/{ownerId}
        /// <summary>
        /// Retrieves specified account from UserAccount in Db
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns>User account from Db</returns>
        [HttpGet("owner/{ownerId}")]
        public async Task<IActionResult> GetAccountAsync(int ownerId)
        {
            // Fetch account of ownerId
            //var account = await context.UserAccounts
            //    .SingleOrDefaultAsync(ua => ua.OwnerId.Equals(ownerId));
            var account = await unitOfWork.userAccountRepository
                .GetUserAccountAsync(ownerId);

            // Return error if account doesn't exist
            // I imagine we'll need to move IsDeleted later if user wants to reactivate account
            if(account == null || account.IsDeleted)
            {
                return NotFound("Account could not be found.");
            }

            // Map account model to resource
            var accountResource = new UserAccountResource
            {
                OwnerId = account.OwnerId,
                TeamId = account.TeamId,
                FirstName = account.FirstName,
                LastName = account.LastName,
                Email = account.Email,
                IsDeleted = account.IsDeleted
            };

            return Ok(accountResource);
        }

        // GET api/accounts/{teamId}
        /// <summary>
        /// Retrieves specified team accounts from UserAccount table in Db
        /// </summary>
        /// <returns>List of team accounts from UserAccount</returns>
        [HttpGet("member/{teamId}")]
        public async Task<IActionResult> GetTeamAccountsAsync(int teamId)
        {
            // Fetch all accounts from the DB asyncronously
            //var accounts = await context.UserAccounts
            //    .Where(ua => ua.TeamId.Equals(teamId))
            //    .ToListAsync();
            var accounts = await unitOfWork.userAccountRepository
                .GetTeamAccountsAsync(teamId);

            // Return error if no team exists
            if(accounts.Count() == 0)
            {
                return NotFound("No accounts in this team.");
            }

            // List of corresponding accounts as resources
            var resourceAccounts = new List<UserAccountResource>();

            // Retrive accounts from database
            foreach (var account in accounts)
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
            }
            // Return account resource
            return Ok(resourceAccounts);
        }

        // PUT api/accounts/{ownerId}
        /// <summary>
        /// Updates an existing account in Db
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="userAccountResource"></param>
        /// <returns>A resource of the updated account</returns>
        [HttpPut("{ownerId}")]
        public async Task<IActionResult> EditAccountAsync(int ownerId, [FromBody]UserAccountResource userAccountResource)
        {
            // How does this validate against the UserAccount model?
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Retrieve ownerId account from database
            //var account = await context.UserAccounts
            //    .SingleOrDefaultAsync(ua => ua.OwnerId == ownerId);
            var account = await unitOfWork.userAccountRepository
                .GetUserAccountAsync(ownerId);

            // No account matches ownerId
            if(account == null)
            {
                return NotFound("No account found matching that ownerId.");
            }

            // Map account resource to model
            account.TeamId = userAccountResource.TeamId;
            account.FirstName = userAccountResource.FirstName;
            account.LastName = userAccountResource.LastName;
            account.Email = userAccountResource.Email;
            account.IsDeleted = userAccountResource.IsDeleted;

            // Save updated account to database
            //await context.SaveChangesAsync();
            await unitOfWork.CompleteAsync();

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