using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SynchroLean.Controllers.Resources;
using SynchroLean.Core.Models;
using SynchroLean.Persistence;
using SynchroLean.Core;
using Microsoft.AspNetCore.Authorization;

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
        /// <param name="createUserAccountResource"></param>
        /// <returns>New account retrieved from Db</returns>
        [HttpPost]
        public async Task<IActionResult> AddUserAccountAsync([FromBody]CreateUserAccountResource createUserAccountResource)
        {
            // How does this validate against the UserAccount model?
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Map account resource to model
            var account = _mapper.Map<UserAccount>(createUserAccountResource);

            // Salt and hash password
            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var saltedPassword = account.Password + salt;
            account.Password = BCrypt.Net.BCrypt.HashPassword(saltedPassword);
            account.Salt = salt;

            // Add model to database and save changes
            await unitOfWork.userAccountRepository.AddAsync(account);
            Task.WaitAll(unitOfWork.CompleteAsync());

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
        [HttpGet("owner/{ownerId}"), Authorize]
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

        // GET api/accounts/owner/{emailAddress}
        /// <summary>
        /// Retrieves a UserAccount with the specified email address
        /// </summary>
        /// <param name="emailAddress">The email address of the user being searched for</param>
        /// <returns>User account from Db</returns>
        [HttpGet("{emailAddress}"), Authorize]
        public async Task<IActionResult> GetAccountByEmailAsync(string emailAddress)
        {
            // Fetch account of ownerId
            var account = await unitOfWork.userAccountRepository
                .GetUserAccountByEmailAsync(emailAddress);

            // Return error if account doesn't exist
            // I imagine we'll need to move IsDeleted later if user wants to reactivate account
            if(account == null || account.IsDeleted)
            {
                return NotFound("Account could not be found.");
            }

            // Return mapped account resource
            return Ok(_mapper.Map<UserAccountResource>(account));
        }

        // PUT api/accounts/{ownerId}
        /// <summary>
        /// Updates an existing account in Db
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="userAccountResource"></param>
        /// <returns>A resource of the updated account</returns>
        [HttpPut("{ownerId}"), Authorize]
        public async Task<IActionResult> EditAccountAsync(int ownerId, [FromBody]UserAccountResource userAccountResource)
        {
            // How does this validate against the UserAccount model?
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tokenOwnerId = Convert.ToInt32(User.FindFirst("OwnerId").Value);
            if (!tokenOwnerId.Equals(ownerId)) 
            {
                return Forbid();
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
            account.FirstName = userAccountResource.FirstName;
            account.LastName = userAccountResource.LastName;
            account.Email = userAccountResource.Email;
            account.IsDeleted = userAccountResource.IsDeleted;

            // Save updated account to database
            await unitOfWork.CompleteAsync();

            // Return mapped resource
            return Ok(_mapper.Map<UserAccountResource>(account));
        }

        /// <summary>
        /// Get all the teams a user is on.
        /// </summary>
        /// <param name="ownerId">The user for which to get teams for.</param>
        /// <returns></returns>
        [HttpGet("teams/{ownerId}"), Authorize]
        public async Task<IActionResult> GetTeamsForAccount(int ownerId)
        {
            // Check if user exists
            var userExists = await unitOfWork.userAccountRepository.UserAccountExists(ownerId);

            // No account matches ownerId
            if (!userExists)
            {
                return NotFound("No account found matching that ownerId.");
            }

            var teams = await unitOfWork.teamMemberRepository.GetAllTeamsForUser(ownerId);
            return Ok(teams.Select(team => _mapper.Map<TeamResource>(team)));
        }
    }
}
