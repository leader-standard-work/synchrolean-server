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

namespace SynchroLean.Controllers
{
    [Route("api/[controller]")]
    public class TeamController : Controller
    {
        private readonly SynchroLeanDbContext context; // Added (DbSet<Team> Teams) to context
        private readonly IMapper _mapper;

        public TeamController(SynchroLeanDbContext context, IMapper mapper)
        {
            this.context = context;
            _mapper = mapper;
        }

        // POST api/team
        [HttpPost]
        public async Task<IActionResult> AddTeamAsync([FromBody]TeamResource teamResource)
        {
            // Validate against the team model
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // Map the team resource to a model
            var teamModel = _mapper.Map<Team>(teamResource);

            // Add the team to context and save changes
            await context.AddAsync(teamModel);
            await context.SaveChangesAsync();

            // Fetch the newly created team from the DB
            teamModel = await context.Teams
                .SingleOrDefaultAsync(tm => tm.Id.Equals(teamModel.Id));
            
            return Ok(_mapper.Map<TeamResource>(teamModel)); // Return newly created mapped team resource to client
        }

        /** 
         * Acts as a get all teams method. The reasoning for this is... if a user
         * is supposed to be able to view aggregate metrics for other teams then
         * they would have to be able to get all the teams. This can be modified
         * for various scenarios... E.G. maybe a team owner can look at other 
         * teams aggregate metrics...
         **/
        // GET api/team
        [HttpGet]
        public async Task<IActionResult> GetTeamsAsync()
        {
            // Fetch all teams from the database
            var teams = await context.Teams.ToListAsync<Team>();

            // List of resource versions of teams
            var resourceTeams = new List<TeamResource>();

            // Map each team to a resource
            teams.ForEach(team => {
                // Add mapped resource to collection
                resourceTeams.Add(_mapper.Map<TeamResource>(team));
            });

            return Ok(resourceTeams); // Return the collection of team resources
        }

        /** 
         * Handler to get the team for the currently logged in user. This is in case 
         * we only want to fetch the team for the person who is currently logged in.
         **/
        // GET api/team/tid
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserTeamAsync(int id)
        {
            // Get the team for the currently logged in user
            var team = await context.Teams
                .SingleOrDefaultAsync(ut => ut.Id.Equals(id));

            // Check to see if a team corresponding to the given team id was found
            if (team == null)
            {
                return NotFound(); // Team wasn't found
            }
           
            return Ok(_mapper.Map<TeamResource>(team)); // Return mapped team to client
        }
    
        // PUT api/team/ownerId/teamId
        [HttpPut("{ownerId}/{teamId}")]
        public async Task<IActionResult> UpdateUserTeamAsync(int ownerId, int teamId, [FromBody]TeamResource teamResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            
            // Fetch an account from the DB asynchronously
            var account = await context.UserAccounts
                .SingleOrDefaultAsync(ua => ua.OwnerId == ownerId);
            
            // Return not found exception if account doesn't exist
            if(account == null)
            {
                return NotFound("account not found");
            } 
            
            // Get the team for the currently logged in user
            var team = await context.Teams
                .SingleOrDefaultAsync(ut => ut.Id.Equals(teamId));

            // Nothing was retrieved, no id match
            if (team == null)
            {
                return NotFound("team not found");
            }

            // Validates team belongs to correct user
            if(team.OwnerId != account.OwnerId)
            {
                return BadRequest("prohibited user does not have edit rights");
            } 

            // Map resource to model
            team.TeamName = teamResource.TeamName;
            team.TeamDescription = teamResource.TeamDescription;
            team.OwnerId = teamResource.OwnerId;

            //this stops default edit team from giving teams to user 0
            if(team.OwnerId == 0)
            {
                team.OwnerId = ownerId;
            }

            // Save updated team to database
            await context.SaveChangesAsync();
            
            // Return mapped team resource
            return Ok(_mapper.Map<UserTaskResource>(team));
        }
    }
}


