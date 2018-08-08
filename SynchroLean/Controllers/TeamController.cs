using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        // POST api/team
        /// <summary>
        /// Adds a new team to the Db asynchronously
        /// </summary>
        /// <param name="teamResource"></param>
        /// <returns>A resource of the new team</returns>
        [HttpPost, Authorize]
        public async Task<IActionResult> AddTeamAsync([FromBody]TeamResource teamResource)
        {
            // Validate against the team model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate that the user creating the team is the assigned owner
            var tokenOwnerId = Convert.ToInt32(User.FindFirst("OwnerId").Value);
            if (!tokenOwnerId.Equals(teamResource.OwnerId)) 
            {
                return Forbid();
            }

            // Map the team resource to a model
            var teamModel = _mapper.Map<Team>(teamResource);

            // Add the team to context and save changes
            await unitOfWork.userTeamRepository.AddAsync(teamModel);
            await unitOfWork.CompleteAsync();

            // Fetch the newly created team from the DB
            var team = await unitOfWork.userTeamRepository
                .GetUserTeamAsync(teamModel.Id);

            return Ok(_mapper.Map<TeamResource>(team)); // Return newly created mapped team resource to client
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
        [HttpGet, Authorize]
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
        /// Get the given team.
        /// </summary>
        /// <param name="teamId">The id for the team to fetch.</param>
        /// <returns>A team resource</returns>
        // GET api/team/tid
        [HttpGet("{teamId}"), Authorize]
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

        /// <summary>
        /// Get a list of all members for a team.
        /// </summary>
        /// <param name="teamId">The team for which to get members.</param>
        /// <returns></returns>
        [HttpGet("members/{teamId}"), Authorize]
        public async Task<IActionResult> GetTeamMembers(int teamId)
        {
            // Get the team for the currently logged in user
            var team = await unitOfWork.userTeamRepository
                .GetUserTeamAsync(teamId);

            // Check to see if a team corresponding to the given team id was found
            if (team == null)
            {
                return NotFound("Couldn't find a team matching that id."); // Team wasn't found
            }

            var teamMembers = await unitOfWork.teamMemberRepository.GetAllUsersForTeam(teamId);
            return Ok(teamMembers.Select(member => _mapper.Map<UserAccountResource>(member)));
        }

        // PUT api/team/ownerId/teamId
        /// <summary>
        /// Updates an existing team in the Db
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="teamId"></param>
        /// <param name="teamResource"></param>
        /// <returns>A resource of updated team</returns>
        [HttpPut("{ownerId}/{teamId}"), Authorize]
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
            if (account == null)
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

            // Validates that team belongs to editing user
            var tokenOwnerId = Convert.ToInt32(User.FindFirst("OwnerId").Value);
            if (!tokenOwnerId.Equals(team.OwnerId)) 
            {
                return Forbid();
            }

            // Map resource to model
            team.TeamName = teamResource.TeamName;
            team.TeamDescription = teamResource.TeamDescription;

            await unitOfWork.teamMemberRepository.ChangeTeamOwnership(teamId, teamResource.OwnerId);

            //this stops default edit team from giving teams to user 0
            if (team.OwnerId == 0)
            {
                team.OwnerId = ownerId;
            }

            // Save updated team to database
            await unitOfWork.CompleteAsync();

            // Return mapped team resource
            return Ok(_mapper.Map<TeamResource>(team));
        }
        /// <summary>
        /// Create a new invite for a user.
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="creatorId"></param>
        /// <param name="teamId"></param>
        /// <returns></returns>
        // PUT api/team/invite/
        [HttpPut("invite/{ownerId}/{creatorId}/{teamId}"), Authorize]
        public async Task<IActionResult> InviteUserToTeamAsync(int ownerId, int creatorId, int teamId)
        {
            var team = await unitOfWork.userTeamRepository.GetUserTeamAsync(teamId);
            if (team == null) return NotFound("No such team");
            //TODO: Check if creator is in the team creator is being invited into
            //blocked by the fact that teams aren't implemented yet
            var tokenOwnerId = Convert.ToInt32(User.FindFirst("OwnerId").Value);
            var creatorIsTeamOwner = team.OwnerId == tokenOwnerId;
            var creator = await unitOfWork.userAccountRepository.GetUserAccountAsync(creatorId);
            if (creator == null) return NotFound("User doesn't exist");
            var owner = await unitOfWork.userAccountRepository.GetUserAccountAsync(ownerId);
            if (owner == null) return NotFound("User doesn't exist");
            await unitOfWork.addUserRequestRepository.AddAsync(
                new AddUserRequest
                {
                    Invitee = owner,
                    Inviter = creator,
                    IsAuthorized = creatorIsTeamOwner,
                    DestinationTeam = team
                });
            await unitOfWork.CompleteAsync();
            return Ok();
        }

        /// <summary>
        /// Accept a user's authorized invite.
        /// </summary>
        /// <param name="addUserRequestId">The invite being accepted.</param>
        /// <param name="inviteeId">The user accepting the invite.</param>
        /// <returns></returns>
        [HttpPut("invite/accept/{addUserRequestId}/{inviteeId}"), Authorize]
        public async Task<IActionResult> AcceptTeamInvite(int addUserRequestId, int inviteeId)
        {
            var invite = await unitOfWork.addUserRequestRepository.GetAddUserRequestAsync(addUserRequestId);
            if (invite == null) return NotFound("No such invite");

            // Validate that the accepting user is the invitee
            var tokenOwnerId = Convert.ToInt32(User.FindFirst("OwnerId").Value);
            if (!tokenOwnerId.Equals(invite.Invitee.OwnerId)) 
            {
                return Forbid();
            }

            await unitOfWork.teamMemberRepository.AddUserToTeam(invite.DestinationTeam.Id, invite.Invitee.OwnerId);
            await unitOfWork.addUserRequestRepository.DeleteAddUserRequestAsync(addUserRequestId);
            await unitOfWork.CompleteAsync();
            return Ok();
        }

        /// <summary>
        /// Reject a specific invitation for your account
        /// </summary>
        /// <param name="addUserRequestId"></param>
        /// <param name="inviteeId"></param>
        /// <returns></returns>
        [HttpPut("invite/reject/{addUserRequestId}/{inviteeId}"), Authorize]
        public async Task<IActionResult> RejectTeamInvite(int addUserRequestId, int inviteeId)
        {
            var invite = await unitOfWork.addUserRequestRepository.GetAddUserRequestAsync(addUserRequestId);
            if (invite == null) return NotFound("No such invite");
            
            // Validate that the rejecting user is the invitee
            var tokenOwnerId = Convert.ToInt32(User.FindFirst("OwnerId").Value);
            if (!tokenOwnerId.Equals(invite.Invitee.OwnerId)) 
            {
                return Forbid();
            }

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
        [HttpPut("invite/rescind/{addUserRequestId}/{creatorId}"), Authorize]
        public async Task<IActionResult> RescindTeamInvite(int addUserRequestId, int creatorId)
        {
            var invite = await unitOfWork.addUserRequestRepository.GetAddUserRequestAsync(addUserRequestId);
            if (invite == null) return NotFound("No such invite");

            // Validate that the rescinding user is the inviter
            var tokenOwnerId = Convert.ToInt32(User.FindFirst("OwnerId").Value);
            if (!tokenOwnerId.Equals(invite.Inviter.OwnerId)) 
            {
                return Forbid();
            }

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
        [HttpPut("invite/authorize/{addUserRequestId}/{ownerId}"), Authorize]
        public async Task<IActionResult> AuthorizeTeamInvite(int addUserRequestId, int ownerId)
        {
            var invite = await unitOfWork.addUserRequestRepository.GetAddUserRequestAsync(addUserRequestId);
            if (invite == null) return NotFound("No such invite");
            // Note: ensure up cascade deletion for team invites, we should be able to assume the team exists here
            var team = await unitOfWork.userTeamRepository.GetUserTeamAsync(invite.DestinationTeam.Id);
            if (team == null) return NotFound("No such team");

            // Validate that the authorizing user is the team owner
            var tokenOwnerId = Convert.ToInt32(User.FindFirst("OwnerId").Value);
            if (!tokenOwnerId.Equals(team.OwnerId)) 
            {
                return Forbid();
            }

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
        [HttpPut("invite/veto/{addUserRequestId}/{ownerId}"), Authorize]
        public async Task<IActionResult> VetoTeamInvite(int addUserRequestId, int ownerId)
        {
            var invite = await unitOfWork.addUserRequestRepository.GetAddUserRequestAsync(addUserRequestId);
            if (invite == null) return NotFound("No such invite");
            // Note: ensure up cascade deletion for team invites, we should be able to assume the team exists here
            var team = await unitOfWork.userTeamRepository.GetUserTeamAsync(invite.DestinationTeam.Id);
            if (team == null) return NotFound("No such team");
            
            // Validate that the vetoing user is the team owner
            var tokenOwnerId = Convert.ToInt32(User.FindFirst("OwnerId").Value);
            if (!tokenOwnerId.Equals(team.OwnerId)) 
            {
                return Forbid();
            }

            await unitOfWork.addUserRequestRepository.DeleteAddUserRequestAsync(addUserRequestId);
            await unitOfWork.CompleteAsync();
            return Ok();
        }

        /// <summary>
        /// Get all the pending invites that can be accepted by a given user.
        /// </summary>
        /// <param name="ownerId">The id for the user.</param>
        /// <returns>All invites that the user may accept.</returns>
        [HttpGet("invite/incoming/accept/{ownerId}"), Authorize]
        public async Task<IActionResult> GetInvitesToAccept(int ownerId)
        {
            var userExists = await unitOfWork.userAccountRepository.UserAccountExists(ownerId);
            if (!userExists) return NotFound("No such user");

            // Validate that the user is fetching their own invites
            var tokenOwnerId = Convert.ToInt32(User.FindFirst("OwnerId").Value);
            if (!tokenOwnerId.Equals(ownerId))
            {
                return Forbid();
            }

            var invites = await unitOfWork.addUserRequestRepository.GetAddUserRequestsPendingAcceptanceAsync(ownerId);
            return Ok(invites.Select(inv => new AddUserRequestResource(inv)));
        }

        /// <summary>
        /// Get all the pending invites a user can authorize.
        /// </summary>
        /// <param name="ownerId">The id for the user.</param>
        /// <returns>All invites that the user may authorize.</returns>
        [HttpGet("invite/incoming/authorize/{ownerId}"), Authorize]
        public async Task<IActionResult> GetInvitesToAuthorize(int ownerId)
        {
            var userExists = await unitOfWork.userAccountRepository.UserAccountExists(ownerId);
            if (!userExists) return NotFound("No such user");

            // Validate that the user is fetching invites waiting for their own authorization
            var tokenOwnerId = Convert.ToInt32(User.FindFirst("OwnerId").Value);
            if (!tokenOwnerId.Equals(ownerId)) 
            {
                return Forbid();
            }

            var invites = await unitOfWork.addUserRequestRepository.GetAddUserRequestsPendingApprovalAsync(ownerId);
            return Ok(invites.Select(inv => new AddUserRequestResource(inv)));
        }

        /// <summary>
        /// Get all the pending invites a user has created and can rescind.
        /// </summary>
        /// <param name="ownerId">The id for the user.</param>
        /// <returns>All invites that the user has created and may rescind.</returns>
        [HttpGet("invite/outgoing/{ownerId}"), Authorize]
        public async Task<IActionResult> GetCreatedInvites(int ownerId)
        {
            var userExists = await unitOfWork.userAccountRepository.UserAccountExists(ownerId);
            if (!userExists) return NotFound("No such user");

            // Validate that the user is fetching their own created invites
            var tokenOwnerId = Convert.ToInt32(User.FindFirst("OwnerId").Value);
            if (!tokenOwnerId.Equals(ownerId)) 
            {
                return Forbid();
            }

            var invites = await unitOfWork.addUserRequestRepository.GetMySentAddUserRequestsAsync(ownerId);
            return Ok(invites.Select(inv => new AddUserRequestResource(inv)));
        }

        /// <summary>
        /// Permit a team to see detailed stats on one of a user's teams.
        /// </summary>
        /// <param name="ownerId">The user granting permissions.</param>
        /// <param name="subjectId">The team for which permissions will be granted.</param>
        /// <param name="objectId">The user's team.</param>
        /// <returns></returns>
        [HttpPut("permissions/grant/{objectId}/{subjectId}/{ownerId}"), Authorize]
        public async Task<IActionResult> PermitTeamToSee(int ownerId, int subjectId, int objectId)
        {
            // -- We don't need to actually check who the user is, but if you want to, uncomment this
            //var ownerExists = await unitOfWork.userAccountRepository.UserAccountExists(ownerId);
            //if (!ownerExists) return NotFound("No such user");
            var subjectExists = await unitOfWork.userTeamRepository.TeamExists(subjectId);
            if (!subjectExists) return NotFound("No such team (subjectId)");
            var objectTeam = await unitOfWork.userTeamRepository.GetUserTeamAsync(objectId);
            if (objectTeam == null) return NotFound("No such team (objectId)");

            // Validate that the user granting permissions owns the object team
            var tokenOwnerId = Convert.ToInt32(User.FindFirst("OwnerId").Value);
            if (!tokenOwnerId.Equals(objectTeam.OwnerId)) 
            {
                return Forbid();
            }

            await unitOfWork.teamPermissionRepository.Permit(subjectId, objectId);
            await unitOfWork.CompleteAsync();
            return Ok();
        }

        /// <summary>
        /// Forbid a team from seeing detailed stats on one of a user's teams.
        /// </summary>
        /// <param name="ownerId">The user revoking permissions.</param>
        /// <param name="subjectId">The team for which permissions will be revoking.</param>
        /// <param name="objectId">The user's team.</param>
        /// <returns></returns>
        [HttpPut("permissions/revoke/{objectId}/{subjectId}/{ownerId}"), Authorize]
        public async Task<IActionResult> ForbidTeamToSee(int ownerId, int subjectId, int objectId)
        {
            // -- We don't need to actually check who the user is, but if you want to, uncomment this
            //var ownerExists = await unitOfWork.userAccountRepository.UserAccountExists(ownerId);
            //if (!ownerExists) return NotFound("No such user");
            var subjectExists = await unitOfWork.userTeamRepository.TeamExists(subjectId);
            if (!subjectExists) return NotFound();
            var objectTeam = await unitOfWork.userTeamRepository.GetUserTeamAsync(objectId);
            if (objectTeam == null) return NotFound("No such team (objectId)");

            // Validate that the user revoking permissions owns the object team
            var tokenOwnerId = Convert.ToInt32(User.FindFirst("OwnerId").Value);
            if (!tokenOwnerId.Equals(objectTeam.OwnerId)) 
            {
                return Forbid();
            }

            await unitOfWork.teamPermissionRepository.Forbid(subjectId, objectId);
            await unitOfWork.CompleteAsync();
            return Ok();
        }        

        /// <summary>
        /// Removes a user from a team, except a team owner (currently)
        /// </summary>
        /// <param name="callerId">The user calling to remove an other user from the team.</param>
        /// <param name="targetId">The user to be removed from the team.</param>
        /// <param name="teamId"> The team the user is too be removed from.</param>
        /// <returns></returns>
        [HttpPut("remove/{callerId}/{targetId}/{teamId}"), Authorize]
        public async Task<IActionResult> RemoveMemberAsync(int callerId, int targetId, int teamId){
        
            var targetTeam = await unitOfWork.userTeamRepository
                .GetUserTeamAsync(teamId);
            
            if (targetTeam == null){
                return NotFound("Not a valid team.");
            }

            // Validate that the user removing another user is either the team owner
            // or they are removing themselves (leaving the team).
            var tokenOwnerId = Convert.ToInt32(User.FindFirst("OwnerId").Value);
            if (!tokenOwnerId.Equals(targetTeam.OwnerId) && !tokenOwnerId.Equals(targetId)) 
            {
                return Forbid();
            }

            if(targetTeam.OwnerId == targetId){
                return BadRequest("Can't remove the team owner");
            }

            await unitOfWork.teamMemberRepository.RemoveUserFromTeam(teamId,targetId);
            await unitOfWork.CompleteAsync();
            return Ok();
        }

        /// <summary>
        ///  Returns a list of tasks for a team
        /// </summary>
        /// <param name="teamId">
        /// <returns></returns>
        [HttpGet("rollup/{teamId}"), Authorize]
        public async Task<IActionResult> TeamRollUpAsync(int teamId){
                        
            var team = await unitOfWork.userTeamRepository
                .GetUserTeamAsync(teamId);

            if (team == null){
                return NotFound("Not a valid team.");
            }

            var tokenOwnerId = Convert.ToInt32(User.FindFirst("OwnerId").Value);
            var userCanSeeTeam = await unitOfWork.teamPermissionRepository.UserIsPermittedToSeeTeam(tokenOwnerId, teamId);
            if (!userCanSeeTeam)
            {
                return Forbid();
            }

            var teamRoster = await unitOfWork.teamMemberRepository.GetAllUsersForTeam(teamId);

            var teamTasks = new List<UserTaskResource>();

            foreach(var member in teamRoster){
                var tasks = await unitOfWork.userTaskRepository
                .GetTasksAsync(member.OwnerId);

                foreach(var task in tasks){
                    if(unitOfWork.todoList.GetUserTodo(member.OwnerId,task.Id) != null
                        /* && task.teamId==teamId*/){
                        teamTasks.Add(_mapper.Map<UserTaskResource>(task));
                    }
                }
            }

        return Ok(teamTasks);      
        }
    }
}


