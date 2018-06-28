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
        public IActionResult AddUserTask([FromBody]UserTaskResource userTaskResource)
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
            context.Add(userTask);
            context.SaveChanges();

            // Retrieve userTask from database
            userTask = context.UserTasks
                .SingleOrDefault(ut => ut.Id == userTask.Id);

            // Map userTask to UserTaskResource
            var outResource = new UserTaskResource {
                Id = userTask.Id,
                Name = userTask.Name,
                Description = userTask.Description,
                IsRecurring = userTask.IsRecurring
            };

            return Ok(outResource);
        }

        // GET api/tasks     (fetch all tasks... will change to async)
        [HttpGet]
        public IActionResult GetTasks()
        {
            /** Once we have a working context we can actually implement this
              *  Something along the lines of:
              *  * var tasks = _context.Tasks.Get()
              *  * Check if the operation succeeded (if not return Bad response) else continue
              *  * Maybe some light formatting of the tasks to beautify them (map to task resource)
              *  * return Ok(tasks)
             **/

            // For now just serve back some synchronous dummy data so we can see the api at work
            var task = new UserTask();
            task.Id = 1;
            task.Name = "Walk the cat...";                    // Dummy task
            task.Description = "Who walks their cat?...";
            task.IsRecurring = true;

            var taskTwo = new UserTask();
            taskTwo.Id = 2;
            taskTwo.Name = "Get rid of the cat.";             // Another dummy task
            taskTwo.Description = "Get rid of the cat so I don't have to walk it anymore.";
            task.IsRecurring = false;
            // Add to simulated collection of tasks
            var tasks = new Collection<UserTask>();
            tasks.Add(task);
            tasks.Add(taskTwo);
            return Ok(tasks); // Return simulated response
        }
    }
}
