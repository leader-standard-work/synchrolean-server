using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SynchroLean.Controllers.Resources;
using SynchroLean.Models;

namespace SynchroLean.Controllers
{
    [Route("api/[controller]")]
    public class TasksController : Controller
    {
        // POST api/tasks
        [HttpPost]
        public IActionResult AddUserTask([FromBody]UserTaskResource userTaskResource)
        {
            // How does this validate against the UserTask model?
            if(!ModelState.IsValid) {
                return BadRequest();
            }

            // Map resource to model
            var userTask = new UserTask();
            userTask.Id = userTaskResource.Id;
            userTask.Name = userTaskResource.Name;
            userTask.Description = userTaskResource.Description;
            userTask.IsRecurring = userTaskResource.IsRecurring;

            var weekdays = new Collection<Weekday>();
            foreach (WeekdayResource weekday in userTaskResource.Weekdays) {
                var newWeekday = new Weekday {
                    Id = weekday.Id,
                    Name = weekday.Name
                };
                weekdays.Add(newWeekday);
            }
            userTask.Weekdays = weekdays;

            // Save userTask to database
            // Retrieve userTask from database
            // Map userTask to UserTaskResource

            return Ok(userTask);
        }

        // GET api/tasks     (fetch all tasks... will change to async)
        [HttpGet]
        public IActionResult GetTasks()
        {
            /** Once we have a working context we can actually implement this
              *  Something along the lines of:
              *  * var tasks = _context.Tasks.Get()
              *  * Check if the operation succeeded (if not return Bad response) else continue
              *  * Maybe some light formatting of the tasks to beautify them
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
