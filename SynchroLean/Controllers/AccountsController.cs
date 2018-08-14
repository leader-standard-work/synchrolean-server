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
using SynchroLean.Extensions;

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

        // POST api/accounts
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

            // Verify email address has valid structure
            string normalizedAddress;
            if (!EmailExtension.TryNormalizeEmail(createUserAccountResource.Email, out normalizedAddress))
            {
                return BadRequest("Not a valid email address!");
            }

            // Salt and hash password
            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var saltedPassword = createUserAccountResource.Password + salt;
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(saltedPassword);

            var existingAccount = await unitOfWork.UserAccountRepository
                .GetUserAccountAsync(createUserAccountResource.Email);

            if (existingAccount != null && existingAccount.IsDeleted)
            {
                // Overwrite existing account information and undelete it.
                existingAccount.FirstName = createUserAccountResource.FirstName;
                existingAccount.LastName = createUserAccountResource.LastName;
                existingAccount.Password = hashedPassword;
                existingAccount.Salt = salt;
                existingAccount.Deleted = null;
            }
            else if (existingAccount == null)
            {
                // Map account resource to model
                var account = _mapper.Map<UserAccount>(createUserAccountResource);

                account.Password = hashedPassword;
                account.Salt = salt;
                account.Email = account.Email.Trim().ToLower();

                // Add model to database and save changes
                await unitOfWork.UserAccountRepository.AddAsync(account);
            }
            else return BadRequest("Account already exists");

            Task.WaitAll(unitOfWork.CompleteAsync());

            // Retrieve account from database
            var createdAccount = await unitOfWork.UserAccountRepository
                .GetUserAccountAsync(createUserAccountResource.Email);
            
            // Return mapped account resource
            return Ok(_mapper.Map<UserAccountResource>(createdAccount));
        }

        // GET api/accounts/{emailAddress}
        /// <summary>
        /// Retrieves a UserAccount with the specified email address
        /// </summary>
        /// <param name="emailAddress">The email address of the user being searched for</param>
        /// <returns>User account from Db</returns>
        [HttpGet("{emailAddress}"), Authorize]
        public async Task<IActionResult> GetAccountAsync(string emailAddress)
        {
            // Verify email address has valid structure
            string normalizedAddress;
            if (!EmailExtension.TryNormalizeEmail(emailAddress, out normalizedAddress))
            {
                return BadRequest("Not a valid email address!");
            }

            // Fetch account of ownerId
            var account = await unitOfWork.UserAccountRepository
                .GetUserAccountAsync(normalizedAddress);

            // Return error if account doesn't exist
            // I imagine we'll need to move IsDeleted later if user wants to reactivate account
            if(account == null || account.IsDeleted)
            {
                return NotFound("Account could not be found.");
            }

            // Return mapped account resource
            return Ok(_mapper.Map<UserAccountResource>(account));
        }

        // PUT api/accounts
        /// <summary>
        /// Updates an existing account in Db
        /// </summary>
        /// <param name="userAccountResource"></param>
        /// <returns>A resource of the updated account</returns>
        [HttpPut, Authorize]
        public async Task<IActionResult> EditAccountAsync([FromBody]UserAccountResource userAccountResource)
        {
            // Verify email address has valid structure
            string normalizedAddress;
            if (!EmailExtension.TryNormalizeEmail(userAccountResource.Email, out normalizedAddress))
            {
                return BadRequest("Not a valid email address!");
            }

            // How does this validate against the UserAccount model?
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tokenOwnerEmail = User.FindFirst("Email").Value;

            // Retrieve ownerId account from database
            var account = await unitOfWork.UserAccountRepository
                .GetUserAccountAsync(tokenOwnerEmail);

            // No account matches ownerId
            if(account == null || account.IsDeleted)
            {
                return NotFound("No account found matching that ownerId.");
            }

            // Verify email is unchanged
            if (account.Email != normalizedAddress.Trim().ToLower())
            {
                return BadRequest("Cannot change email address.");
            }

            // See UserTask PUT for issue of mapping back to UserAccountResource
            // Map account resource to model
            account.FirstName = userAccountResource.FirstName;
            account.LastName = userAccountResource.LastName;
            account.Email = normalizedAddress;
            if (userAccountResource.IsDeleted) 
            {
                account.Delete();
            }

            // Save updated account to database
            Task.WaitAll(unitOfWork.CompleteAsync());

            // Return mapped resource
            return Ok(_mapper.Map<UserAccountResource>(account));
        }

        // GET api/accounts/teams/{emailAddress}
        /// <summary>
        /// Get all the teams a user is on.
        /// </summary>
        /// <param name="emailAddress">The user for which to get teams for.</param>
        /// <returns></returns>
        [HttpGet("teams/{emailAddress}"), Authorize]
        public async Task<IActionResult> GetTeamsForAccount(string emailAddress)
        {
            // Verify email address has valid structure
            string normalizedAddress;
            if (!EmailExtension.TryNormalizeEmail(emailAddress, out normalizedAddress))
            {
                return BadRequest("Not a valid email address!");
            }

            // Check if user exists
            var account = await unitOfWork.UserAccountRepository.GetUserAccountAsync(normalizedAddress);

            // No account matches ownerId
            if (account == null || account.IsDeleted)
            {
                return NotFound("No account found matching that ownerId.");
            }

            var teams = await unitOfWork.TeamMemberRepository.GetAllTeamsForUser(normalizedAddress);
            return Ok(teams.Select(team => _mapper.Map<TeamResource>(team)));
        }
    }
}
