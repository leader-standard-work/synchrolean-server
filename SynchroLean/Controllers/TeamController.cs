using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SynchroLean.Controllers.Resources;
using SynchroLean.Core.Models;
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

        public TeamController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
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
            var teamModel = new Team
            {
                OwnerId = teamResource.OwnerId,
                TeamName = teamResource.TeamName,
                TeamDescription = teamResource.TeamDescription
            };

            // Add the team to context and save changes
            await unitOfWork.userTeamRepository.AddAsync(teamModel);
            await unitOfWork.CompleteAsync();

            // Fetch the newly created team from the DB
            teamModel = await unitOfWork.userTeamRepository
                .GetUserTeamAsync(teamModel.Id);

            // Create resource to serve back to client
            var outResource = new TeamResource
            {
                Id = teamModel.Id,
                OwnerId = teamModel.OwnerId,
                TeamName = teamModel.TeamName,
                TeamDescription = teamModel.TeamDescription
            };
            return Ok(outResource); // Return newly created team resource to client
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
                var rTeam = new TeamResource
                {
                    Id = team.Id,
                    OwnerId = team.OwnerId,
                    TeamName = team.TeamName,
                    TeamDescription = team.TeamDescription
                };
                // Add resource to collection
                resourceTeams.Add(rTeam);
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

            // Team was found so map that team to a team resource
            var teamResource = new TeamResource
            {
                Id = team.Id,
                OwnerId = team.OwnerId,
                TeamName = team.TeamName,
                TeamDescription = team.TeamDescription
            };
            return Ok(teamResource); // Return team to client
        }

        /// <summary>
        /// Updates an existing team in the Db
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="teamId"></param>
        /// <param name="teamResource"></param>
        /// <returns>A resource of updated team</returns>
        // PUT api/team/ownerId/teamId
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

            // Map team to TeamResource
            var outResource = new TeamResource
            {
                Id = team.Id,
                TeamName = team.TeamName,
                TeamDescription = team.TeamDescription,
                OwnerId = team.OwnerId
            };
            
            return Ok(outResource);
        }
        /// <summary>
        /// Create a new invite for a user.
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="creatorId"></param>
        /// <param name="teamId"></param>
        /// <returns></returns>
        // PUT api/team/invite/teamId
        [HttpPut("invite/{ownerId}/{creatorId}/{teamId}")]
        public async Task<IActionResult> InviteUserToTeamAsync(int ownerId, int creatorId, int teamId)
        {
            var teamExists = await unitOfWork.userTeamRepository.TeamExists(teamId);
            if (!teamExists) return NotFound();
            var creatorExists = await unitOfWork.userAccountRepository.UserAccountExists(creatorId);
            if (!creatorExists) return NotFound();
            var team = await unitOfWork.userTeamRepository.GetUserTeamAsync(teamId);
            //TODO: Check if creator is in the team creator is being invited into
            //blocked by the fact that teams aren't implemented yet
            var creatorIsTeamOwner = team.OwnerId == creatorId;
            var creator = await unitOfWork.userAccountRepository.GetUserAccountAsync(creatorId);
            var owner = await unitOfWork.userAccountRepository.GetUserAccountAsync(ownerId);
            await unitOfWork.addUserRequestRepository.AddAsync(
                new AddUserRequest
                {
                    Invitee = owner,
                    Inviter = creator,
                    IsAuthorized = creatorIsTeamOwner,
                    DestinationTeam = team
                });
            return Ok();
        }

        /// <summary>
        /// Reject a specific invitation for your account
        /// </summary>
        /// <param name="addUserRequestId"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        [HttpPut("invite/reject/{addUserRequestId}/{creatorId}")]
        public async Task<IActionResult> RejectTeamInvite(int addUserRequestId, int ownerId)
        {
            var inviteExists = await unitOfWork.addUserRequestRepository.AddUserRequestExists(addUserRequestId);
            if (!inviteExists) return NotFound();
            var invite = await unitOfWork.addUserRequestRepository.GetAddUserRequestAsync(addUserRequestId);
            if (!(invite.Invitee.OwnerId == ownerId)) return Forbid();
            await unitOfWork.addUserRequestRepository.DeleteAddUserRequestAsync(addUserRequestId);
            await unitOfWork.CompleteAsync();
            return Ok();
        }

        /// <summary>
        /// Rescind an invitation you gave someone.
        /// </summary>
        /// <param name="addUserRequestId"></param>
        /// <param name="creatorId"></param>
        /// <returns></returns>
        [HttpPut("invite/rescind/{addUserRequestId}/{creatorId}")]
        public async Task<IActionResult> RescindTeamInvite(int addUserRequestId, int creatorId)
        {
            var inviteExists = await unitOfWork.addUserRequestRepository.AddUserRequestExists(addUserRequestId);
            if (!inviteExists) return NotFound();
            var invite = await unitOfWork.addUserRequestRepository.GetAddUserRequestAsync(addUserRequestId);
            if (!(invite.Inviter.OwnerId == creatorId)) return Forbid();
            await unitOfWork.addUserRequestRepository.DeleteAddUserRequestAsync(addUserRequestId);
            await unitOfWork.CompleteAsync();
            return Ok();
        }

        /// <summary>
        /// Authorize an invitation by a team member
        /// </summary>
        /// <param name="addUserRequestId"></param>
        /// <param name="creatorId"></param>
        /// <returns></returns>
        [HttpPut("invite/authorize/{addUserRequestId}/{ownerId}")]
        public async Task<IActionResult> AuthorizeTeamInvite(int addUserRequestId, int ownerId)
        {
            var inviteExists = await unitOfWork.addUserRequestRepository.AddUserRequestExists(addUserRequestId);
            if (!inviteExists) return NotFound();
            var invite = await unitOfWork.addUserRequestRepository.GetAddUserRequestAsync(addUserRequestId);
            // Note: ensure up cascade deletion for team invites, we should be able to assume the team exists here
            var teamExists = await unitOfWork.userTeamRepository.TeamExists(invite.DestinationTeam.Id);
            if (!teamExists) return NotFound();
            var team = await unitOfWork.userTeamRepository.GetUserTeamAsync(invite.DestinationTeam.Id);
            if (!(team.OwnerId == ownerId)) return Forbid();
            invite.IsAuthorized = true;
            await unitOfWork.CompleteAsync();
            return Ok();
        }

        /// <summary>
        /// Veto an invitation by a team member
        /// </summary>
        /// <param name="addUserRequestId"></param>
        /// <param name="creatorId"></param>
        /// <returns></returns>
        [HttpPut("invite/veto/{addUserRequestId}/{ownerId}")]
        public async Task<IActionResult> VetoTeamInvite(int addUserRequestId, int ownerId)
        {
            var inviteExists = await unitOfWork.addUserRequestRepository.AddUserRequestExists(addUserRequestId);
            if (!inviteExists) return NotFound();
            var invite = await unitOfWork.addUserRequestRepository.GetAddUserRequestAsync(addUserRequestId);
            // Note: ensure up cascade deletion for team invites, we should be able to assume the team exists here
            var teamExists = await unitOfWork.userTeamRepository.TeamExists(invite.DestinationTeam.Id);
            if (!teamExists) return NotFound();
            var team = await unitOfWork.userTeamRepository.GetUserTeamAsync(invite.DestinationTeam.Id);
            if (!(team.OwnerId == ownerId)) return Forbid();
            await unitOfWork.addUserRequestRepository.DeleteAddUserRequestAsync(addUserRequestId);
            await unitOfWork.CompleteAsync();
            return Ok();
        }
    }
}


