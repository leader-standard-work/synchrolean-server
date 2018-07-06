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
    /// <summary>
    /// This class handles HTTP requests for tasks
    /// </summary>
    [Route("api/[controller]")]
    public class TasksController : Controller
    {
        private readonly SynchroLeanDbContext context;
        private readonly IMapper _mapper;
        public TasksController(SynchroLeanDbContext context, IMapper mapper)
        {
            this.context = context;
            _mapper = mapper;
        }

        // POST api/tasks/ownerId
        /// <summary>
        /// Adds new task to DB
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="userTaskResource"></param>
        /// <returns>
        /// New task retrieved from DB
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> AddUserTaskAsync([FromBody]UserTaskResource userTaskResource)
        {
            // How does this validate against the UserTask model?
            if(!ModelState.IsValid) {
                return BadRequest();
            }

            // Map object from UserTaskResource into UserTask
            var userTask = _mapper.Map<UserTask>(userTaskResource);

            // Save userTask to database
            await context.AddAsync(userTask);
            await context.SaveChangesAsync();

            // Retrieve userTask from database
            userTask = await context.UserTasks
                .SingleOrDefaultAsync(ut => ut.Id == userTask.Id);

            // Return mapped resource
            return Ok(_mapper.Map<UserTaskResource>(userTask));
        }

        // GET api/tasks/ownerId
        /// <summary>
        /// Retrieves a users tasks
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns>
        /// List of a users tasks
        /// </returns>
        [HttpGet("{ownerId}")]
        public async Task<IActionResult> GetTasksAsync(int ownerId)
        {
            // Fetch all tasks from the DB asyncronously
            var tasks = await context.UserTasks
                .Where(ut => ut.OwnerId.Equals(ownerId))
                .ToListAsync();

            // List of corresponding tasks as resources
            var resourceTasks = new List<UserTaskResource>();

            // Map each task to a corresponding resource
            tasks.ForEach(task =>
            {
                // Add mapped resource to resources list
                resourceTasks.Add(_mapper.Map<UserTaskResource>(task));
            });
            return Ok(resourceTasks); // List of UserTaskResources 200OK
        }

        // PUT api/tasks/ownerId/taskId
        /// <summary>
        /// Updates a users task
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="taskId"></param>
        /// <param name="userTaskResource"></param>
        /// <returns>
        /// Updated user task
        /// </returns>
        [HttpPut("{ownerId}/{taskId}")]
        public async Task<IActionResult> EditUserTaskAsync(int ownerId, int taskId, [FromBody]UserTaskResource userTaskResource)
        {
            // How does this validate against the UserTask model?
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
                return NotFound();
            } 
            
            // Retrieves task from UserTasks table
            var task = await context.UserTasks
                .SingleOrDefaultAsync(ut => ut.Id == taskId);

            // Nothing was retrieved, no id match
            if (task == null)
            {
                return NotFound();
            }

            // Validates task belongs to correct user
            if(task.OwnerId != account.OwnerId)
            {
                return BadRequest();
            } 

            // Don't know if it's possible to AutoMap without creating a new model
            // This doesn't work but I'm trying to do something along this line
            //task = _mapper.Map<UserTask>(userTaskResource);

            // Map resource to model
            task.Name = userTaskResource.Name;
            task.Description = userTaskResource.Description;
            task.IsRecurring = userTaskResource.IsRecurring;
            task.Weekdays = userTaskResource.Weekdays;
            if (!task.IsCompleted && userTaskResource.IsCompleted) {
                // We'll need to think about timezones here
                task.CompletionDate = DateTime.Now;
            }
            task.IsCompleted = userTaskResource.IsCompleted;
            task.IsRemoved = userTaskResource.IsRemoved;
            task.OwnerId = userTaskResource.OwnerId;
            
            // Save updated userTask to database
            await context.SaveChangesAsync();
            
            // Return mapped resource
            return Ok(_mapper.Map<UserTaskResource>(task));
        }

        /// <summary>
        /// Get the completion rate for a user.
        /// </summary>
        /// <param name="ownerId">The key to identify the owner.</param>
        /// <returns>The proportion (between 0 and 1) of tasks completed.</returns>
        [HttpGet("metrics/user/{ownerId}")]
        public async Task<IActionResult> GetUserCompletionRate(int ownerId)
        {
            //Check if user exists
            var userExists = await context.UserAccounts.AnyAsync(user => user.OwnerId == ownerId);
            //User doesn't exist
            if (!userExists) return NotFound();
            //User exists
            var userTasks = await
                            (
                                from task in context.UserTasks
                                where task.OwnerId == ownerId
                                select task.IsCompleted ? 1.0 : 0.0
                            ).ToListAsync();
            if (userTasks.Count > 0)
            {
                return Ok(userTasks.Average());
            }
            else
            {
                //NaN or 1 are the sensible values here, depending on interpretation
                //If it is a mean, the empty average is 0/0, or NaN
                //If it is a question about if the user completed all their tasks, then
                // vacuously they did because they had none.
                //Provisionally, I am using NaN, because it is distinct from 1
                return Ok(Double.NaN);
            }
        }

        /// <summary>
        /// See how much of their tasks a team has completed.
        /// </summary>
        /// <param name="id">The key to identify the team.</param>
        /// <returns>The proportion (between 0 and 1) of tasks completed.</returns>
        [HttpGet("metrics/team/{Id}")]
        public async Task<IActionResult> GetTeamCompletionRate(int id)
        {

            //Check if team exists
            var teamExists = await context.Teams.AnyAsync(team => team.Id == id);
            //Team doesn't exist
            if (!teamExists) return NotFound();
            //Team does exist
            var groupTasks = await
                (
                    from task in context.UserTasks
                    join member in (from user in context.UserAccounts
                                    where user.TeamId == id
                                    select user.OwnerId)
                    on task.OwnerId equals member
                    select task.IsCompleted ? 1.0 : 0.0
                ).ToListAsync();
            //Team has tasks
            if(groupTasks.Count > 0)
            {
                return Ok(groupTasks.Average());
            }
            else
            {
                //NaN or 1 are the sensible values here, depending on interpretation
                //If it is a mean, the empty average is 0/0, or NaN
                //If it is a question about if the user completed all their tasks, then
                // vacuously they did because they had none.
                //Provisionally, I am using NaN, because it is distinct from 1
                return Ok(Double.NaN);
            }
        }
    }
}
