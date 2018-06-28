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
    }
}
