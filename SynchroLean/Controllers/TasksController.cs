using System;
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
    public class TasksController : Controller
    {

        private readonly SynchroLeanDbContext context;

        public TasksController(SynchroLeanDbContext context)
        {
            this.context = context;    
        }

        // POST api/tasks
        [HttpPost]
        public async Task<IActionResult> AddUserTaskAsync([FromBody]UserTaskResource userTaskResource)
        {
            // How does this validate against the UserTask model?
            if(!ModelState.IsValid) {
                return BadRequest();
            }

            // Map resource to model
            var userTask = new UserTask {
                Id = userTaskResource.Id,
                Name = userTaskResource.Name,
                Description = userTaskResource.Description,
                IsRecurring = userTaskResource.IsRecurring
            };

            // Save userTask to database
            await context.AddAsync(userTask);
            await context.SaveChangesAsync();

            // Retrieve userTask from database
            userTask = await context.UserTasks
                .SingleOrDefaultAsync(ut => ut.Id == userTask.Id);

            // Map userTask to UserTaskResource
            var outResource = new UserTaskResource {
                Id = userTask.Id,
                Name = userTask.Name,
                Description = userTask.Description,
                IsRecurring = userTask.IsRecurring
            };

            return Ok(outResource);
        }

        // GET api/tasks
        [HttpGet]
        public async Task<IActionResult> GetTasksAsync()
        {
            // Fetch all tasks from the DB asyncronously
            var tasks = await context.UserTasks.ToListAsync<UserTask>();

            // List of corresponding tasks as resources
            var resourceTasks = new List<UserTaskResource>();

            // Map each task to a corresponding resource
            tasks.ForEach(task =>
            {
                // Create resource from model
                var resource = new UserTaskResource {
                    Id = task.Id,
                    Name = task.Name,
                    Description = task.Description,
                    IsRecurring = task.IsRecurring
                };
                // Add to resources list
                resourceTasks.Add(resource);
            });
            return Ok(resourceTasks); // List of UserTaskResources 200OK
        }
    }
}
