﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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

        public TeamController(SynchroLeanDbContext context)
        {
            this.context = context;
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
            var teamModel = new Team
            {
                Id = teamResource.Id,
                OwnerId = teamResource.OwnerId,
                TeamName = teamResource.TeamName,
                TeamDescription = teamResource.TeamDescription
            };

            // Add the team to context and save changes
            await context.AddAsync(teamModel);
            await context.SaveChangesAsync();

            // Fetch the newly created team from the DB
            teamModel = await context.Teams
                .SingleOrDefaultAsync(tm => tm.Id.Equals(teamModel.Id));

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
                var rTeam = new TeamResource
                {
                    Id = team.Id,
                    OwnerId = team.OwnerId,
                    TeamName = team.TeamName,
                    TeamDescription = team.TeamDescription
                };
                // Add resource to collection
                resourceTeams.Add(rTeam);
            });
            return Ok(resourceTeams); // Return the collection of team resources
        }

        /** 
         * Handler to get the team for the currently logged in user. This is in case 
         * we only want to fetch the team for the person who is currently logged in.
         **/
        // GET api/team/tid
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserTeamAsync(int tid)
        {
            // Get the team for the currently logged in user
            var team = await context.Teams
                .SingleOrDefaultAsync(ut => ut.Id.Equals(tid));

            // Check to see if a team corresponding to the given team id was found
            if (team == null)
            {
                return NotFound(); // Team wasn't found
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
    }
}


