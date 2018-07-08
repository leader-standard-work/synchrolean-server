using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
        private readonly IUnitOfWork unitOfWork;
        
        private readonly IMapper _mapper;

        public AccountsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;    
            _mapper = mapper;
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
            var account = _mapper.Map<UserAccount>(userAccountResource);

            // Add model to database and save changes
            await unitOfWork.userAccountRepository.AddAsync(account);
            await unitOfWork.CompleteAsync();

            // Retrieve account from database
            var accountModel = await unitOfWork.userAccountRepository
                .GetUserAccountAsync(account.OwnerId);
            
            // Return mapped account resource
            return Ok(_mapper.Map<UserAccountResource>(account));
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
            var account = await unitOfWork.userAccountRepository
                .GetUserAccountAsync(ownerId);

            // Return error if account doesn't exist
            // I imagine we'll need to move IsDeleted later if user wants to reactivate account
            if(account == null || account.IsDeleted)
            {
                return NotFound("Account could not be found.");
            }

            // Return mapped account resource
            return Ok(_mapper.Map<UserAccountResource>(account));
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
            var accounts = await unitOfWork.userAccountRepository
                .GetTeamAccountsAsync(teamId);

            // Return error if no team exists
            if(accounts.Count() == 0)
            {
                return NotFound("No accounts in this team.");
            }

            // List of corresponding accounts as resources
            var outResources = new List<UserAccountResource>();

            // Retrive accounts from database
            foreach (var account in accounts)
            {
                // Map account model to resource
                outResources.Add(_mapper.Map<UserAccountResource>(account));
            }
            
            // Return account resources
            return Ok(outResources);
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
            var account = await unitOfWork.userAccountRepository
                .GetUserAccountAsync(ownerId);

            // No account matches ownerId
            if(account == null)
            {
                return NotFound("No account found matching that ownerId.");
            }

            // See UserTask PUT for issue of mapping back to UserAccountResource
            // Map account resource to model
            account.TeamId = userAccountResource.TeamId;
            account.FirstName = userAccountResource.FirstName;
            account.LastName = userAccountResource.LastName;
            account.Email = userAccountResource.Email;
            account.IsDeleted = userAccountResource.IsDeleted;

            // Save updated account to database
            await unitOfWork.CompleteAsync();

            // Return mapped resource
            return Ok(_mapper.Map<UserAccountResource>(account));
        }
    }
}