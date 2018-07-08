using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// This class handles HTTP requests for teams
    /// </summary>
    [Route("api/[controller]")]
    public class TeamController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        public TeamController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Adds a new team to the Db asynchronously
        /// </summary>
        /// <param name="teamResource"></param>
        /// <returns>A resource of the new team</returns>
        // POST api/team
        [HttpPost]
        public async Task<IActionResult> AddTeamAsync([FromBody]TeamResource teamResource)
        {
            // Validate against the team model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Map the team resource to a model
            var teamModel = _mapper.Map<Team>(teamResource);

            // Add the team to context and save changes
            await unitOfWork.userTeamRepository.AddAsync(teamModel);
            await unitOfWork.CompleteAsync();

            // Fetch the newly created team from the DB
            teamModel = await unitOfWork.userTeamRepository
                .GetUserTeamAsync(teamModel.Id);
            
            return Ok(_mapper.Map<TeamResource>(teamModel)); // Return newly created mapped team resource to client
        }

        /// <summary>
        /// Acts as a get all teams method. The reasoning for this is... if a user
        /// is supposed to be able to view aggregate metrics for other teams then
        /// they would have to be able to get all the teams. This can be modified
        /// for various scenarios... E.G. maybe a team owner can look at other 
        /// teams aggregate metrics...
        /// </summary>
        /// <returns>A list of all teams</returns>
        // GET api/team
        [HttpGet]
        public async Task<IActionResult> GetTeamsAsync()
        {
            // Fetch all teams from the database
            var teams = await unitOfWork.userTeamRepository
                .GetAllTeamsAsync();

            // List of resource versions of teams
            var resourceTeams = new List<TeamResource>();

            // Map each team to a resource
            foreach (var team in teams)
            { 
                resourceTeams.Add(_mapper.Map<TeamResource>(team));
            }

            return Ok(resourceTeams); // Return the collection of team resources
        }

        /// <summary>
        /// Handler to get the team for the currently logged in user. This is in case 
        /// we only want to fetch the team for the person who is currently logged in.
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns>A team resource</returns>
        // GET api/team/tid
        [HttpGet("{teamId}")]
        public async Task<IActionResult> GetUserTeamAsync(int teamId)
        {
            // Get the team for the currently logged in user
            var team = await unitOfWork.userTeamRepository
                .GetUserTeamAsync(teamId);

            // Check to see if a team corresponding to the given team id was found
            if (team == null)
            {
                return NotFound("Couldn't find a team matching that id."); // Team wasn't found
            }
           
            return Ok(_mapper.Map<TeamResource>(team)); // Return mapped team to client
        }

        // PUT api/team/ownerId/teamId
        /// <summary>
        /// Updates an existing team in the Db
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="teamId"></param>
        /// <param name="teamResource"></param>
        /// <returns>A resource of updated team</returns>
        [HttpPut("{ownerId}/{teamId}")]
        public async Task<IActionResult> UpdateUserTeamAsync(int ownerId, int teamId, [FromBody]TeamResource teamResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Fetch an account from the DB asynchronously
            var account = await unitOfWork.userAccountRepository
                .GetUserAccountAsync(ownerId);

            // Return not found exception if account doesn't exist
            if(account == null)
            {
                return NotFound("Couldn't find an account matching that ownerId.");
            }

            // Get the team for the currently logged in user
            var team = await unitOfWork.userTeamRepository
                .GetUserTeamAsync(teamId);

            // Nothing was retrieved, no id match
            if (team == null)
            {
                return NotFound("Couldn't find a team matching that teamId.");
            }

            // Validates team belongs to correct user
            if(team.OwnerId != account.OwnerId)
            {
                return BadRequest("prohibited user does not have edit rights");
            } 

            // Map resource to model
            team.TeamName = teamResource.TeamName;
            team.TeamDescription = teamResource.TeamDescription;
            team.OwnerId = teamResource.OwnerId; // Should we really be changing the owner id?

            //this stops default edit team from giving teams to user 0
            if(team.OwnerId == 0)
            {
                team.OwnerId = ownerId;
            }

            // Save updated team to database
            await unitOfWork.CompleteAsync();
            
            // Return mapped team resource
            return Ok(_mapper.Map<UserTaskResource>(team));
        }
    }
}


