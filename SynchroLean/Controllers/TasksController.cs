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
    }
}
