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
    /// This class handles HTTP requests for tasks
    /// </summary>
    [Route("api/[controller]")]
    public class TasksController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public TasksController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Map a task to a resource
        /// </summary>
        /// <param name="task">The task to be mapped</param>
        /// <returns>A UserTaskResource representing it</returns>
        protected UserTaskResource mapToTaskResource(UserTask task)
        {
            var mapped = _mapper.Map<UserTaskResource>(task);
            var todo = task.Todo;
            mapped.IsCompleted = todo != null && todo.IsCompleted;
            mapped.CompletionDate = todo != null ? todo.Completed : null;
            return mapped;
        }

        // POST api/tasks
        /// <summary>
        /// Adds new task to DB
        /// </summary>
        /// <param name="userTaskResource"></param>
        /// <returns>New task retrieved from DB</returns>
        [HttpPost, Authorize]
        public async Task<IActionResult> AddUserTaskAsync([FromBody]UserTaskResource userTaskResource)
        {
            // How does this validate against the UserTask model?
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tokenOwnerEmail = User.FindFirst("Email").Value;
            if (!tokenOwnerEmail.Equals(userTaskResource.OwnerEmail)) 
            {
                return Forbid();
            }

            // Map object from UserTaskResource into UserTask
            var userTask = _mapper.Map<UserTask>(userTaskResource);

            // Save userTask to database
            await unitOfWork.UserTaskRepository.AddAsync(userTask);
            await unitOfWork.TodoRepository.AddTodoAsync(userTask.Id);

            // Saves changes to database (Errors if `await` is used on this method)
            Task.WaitAll(unitOfWork.CompleteAsync());

            // Retrieve userTask from database
            userTask = await unitOfWork.UserTaskRepository
                .GetTaskAsync(userTask.Id);

            // Return mapped resource
            return Ok(mapToTaskResource(userTask));
        }

        // GET api/tasks/{emailAddress}
        /// <summary>
        /// Retrieves a users tasks
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns>List of a users tasks</returns>
        [HttpGet("{emailAddress}"), Authorize]
        public async Task<IActionResult> GetTasksAsync(string emailAddress)
        {
            var tokenOwnerEmail = User.FindFirst("Email").Value;
            var tasks = await unitOfWork.UserTaskRepository
                .GetTasksAsync(emailAddress);
            IEnumerable<UserTask> visibleTasks;
            if (tokenOwnerEmail != emailAddress)
            {
                var teamsCanSee = await unitOfWork.TeamPermissionRepository.GetTeamIdsUserIdSees(emailAddress);
                visibleTasks = tasks.Where(task => task.TeamId != null && teamsCanSee.Contains((int)task.TeamId));
            }
            else visibleTasks = tasks;
            // List of corresponding tasks as resources
            var resourceTasks = new List<UserTaskResource>();

            // Map each task to a corresponding resource
            foreach (var task in visibleTasks)
            {
                // Add mapped resource to resources list
                if(!task.IsDeleted){
                    resourceTasks.Add(mapToTaskResource(task));
                }
            }
                
            return Ok(resourceTasks); // List of UserTaskResources 200OK
        }

        // GET api/tasks/team/{emailAddress}/{teamId}
        /// <summary>
        /// Retrieves a users tasks
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="teamId"></param>
        /// <returns>List of a users tasks</returns>
        [HttpGet("team/{emailAddress}/{teamId}"), Authorize]
        public async Task<IActionResult> GetTasksForTeamAsync(string emailAddress, int teamId)
        {
            var tokenOwnerEmail = User.FindFirst("Email").Value;
            var userCanSee = await unitOfWork.TeamPermissionRepository.UserIsPermittedToSeeTeam(tokenOwnerEmail, teamId);
            if (!userCanSee) return Forbid();
            var tasks = await unitOfWork.UserTaskRepository
                .GetTasksAsync(emailAddress);
            var visibleTasks = tasks.Where(task => task.TeamId != null && task.TeamId == teamId);
            // List of corresponding tasks as resources
            var resourceTasks = new List<UserTaskResource>();

            // Map each task to a corresponding resource
            foreach (var task in visibleTasks)
            {
                // Add mapped resource to resources list
                if (!task.IsDeleted)
                {
                    resourceTasks.Add(mapToTaskResource(task));
                }
            }

            return Ok(resourceTasks); // List of UserTaskResources 200OK
        }

        // GET api/tasks/{emailAddress}
        /// <summary>
        /// Retrieves the users tasks for current day
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns>A list of current days tasks</returns>
        [HttpGet("todo/{emailAddress}"), Authorize]
        public async Task<IActionResult> GetTodosAsync(string emailAddress)
        {
            // Check that account exists (can be removed with auth??)
            var account = await unitOfWork.UserAccountRepository
                .GetUserAccountAsync(emailAddress);

            if(account == null)
            {
                return NotFound("Account was not found!");
            }

            // Get todos from Db asynchronously
            var todos = await unitOfWork.TodoRepository.GetTodoListAsync(emailAddress);

            // Count check might be unnecessary 
            if(todos == null || todos.Count() == 0)
            {
                return NotFound("No Task found for today");
            }

            // Return current days tasks
            var taskResources = new List<UserTaskResource>();

            foreach(var todo in todos)
            {
                taskResources.Add(mapToTaskResource(todo.Task));
            }
            
            return Ok(taskResources);
        }

        // GET api/tasks/{emailAddress}/{taskId}
        /// <summary>
        /// Gets a single task from user task
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="taskId"></param>
        /// <returns>A task specified by taskId</returns>
        [HttpGet("{emailAddress}/{taskId}"), Authorize]
        public async Task<IActionResult> GetTaskAsync(string emailAddress, int taskId)
        {
            // Check that account exists
            if(await unitOfWork.UserAccountRepository.GetUserAccountAsync(emailAddress) == null)
            {
                return NotFound("Account not found!");
            }

            // Retrieve task
            var task = await unitOfWork.UserTaskRepository.GetTaskAsync(taskId);

            // Check if task exists
            if(task == null || task.IsDeleted)
            {
                return NotFound("Task not found!");
            }
            else 
            {
                return Ok(mapToTaskResource(task));
            }
        }

        // PUT api/tasks/{taskId}
        /// <summary>
        /// Updates a users task
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="userTaskResource"></param>
        /// <returns>Updated user task</returns>
        [HttpPut("{taskId}"), Authorize]
        public async Task<IActionResult> EditUserTaskAsync(int taskId, [FromBody]UserTaskResource userTaskResource)
        {
            // How does this validate against the UserTask model?
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var tokenOwnerEmail = User.FindFirst("Email").Value;
            if (!tokenOwnerEmail.Equals(userTaskResource.OwnerEmail)) 
            {
                return Forbid();
            }

            // Fetch an account from the DB asynchronously
            var account = await unitOfWork.UserAccountRepository
                .GetUserAccountAsync(tokenOwnerEmail);

            // Return not found exception if account doesn't exist
            if(account == null)
            {
                return NotFound("Couldn't find account matching that ownerId.");
            }

            // Retrieves task from UserTasks table
            var task = await unitOfWork.UserTaskRepository
                .GetTaskAsync(taskId);

            // Nothing was retrieved, no id match
            if (task == null || task.IsDeleted)
            {
                return NotFound("Task couldn't be found.");
            }

            // Validates task belongs to correct user
            if(task.OwnerEmail != account.Email)
            {
                return BadRequest("Task does not belong to this account.");
            }

            //Check if a todo for that task exists
            var todo = await unitOfWork.TodoRepository.GetTodo(taskId);
            var todoExists = todo != null;
            //Complete the task if needed
            if(todoExists)
            {
                if (userTaskResource.IsCompleted && !todo.IsCompleted)
                    await unitOfWork.TodoRepository.CompleteTodoAsync(todo.TaskId);
                else if (!userTaskResource.IsCompleted && todo.IsCompleted)
                    await unitOfWork.TodoRepository.UndoCompleteTodoAsync(todo.TaskId);
            }
            //Delete the task if needed
            //Remove the todo if needed
            if(userTaskResource.IsDeleted)
            {
                await unitOfWork.TodoRepository.RemoveTodosAsync(taskId);
            }

            // Map resource to model
            task.Name = userTaskResource.Name;
            task.Description = userTaskResource.Description;
            task.IsRecurring = userTaskResource.IsRecurring;
            task.Weekdays = userTaskResource.Weekdays;
            if (userTaskResource.IsDeleted)
            {
                task.Delete();
            }
            task.OwnerEmail = userTaskResource.OwnerEmail;
            task.TeamId = userTaskResource.TeamId;

            //Refresh the todo list 
            await unitOfWork.TodoRepository.RefreshTodo(taskId);
            
            // Save updated userTask to database
            Task.WaitAll(unitOfWork.CompleteAsync());

            // Return mapped resource
            return Ok(mapToTaskResource(task));
        }

        // GET api/tasks/metrics/user/{emailAddress}/{startDate}/{endDate}
        /// <summary>
        /// Get the completion rate for a user.
        /// </summary>
        /// <param name="emailAddress">The key to identify the owner.</param>
        /// <param name="startDate">Beginning date range</param>
        /// <param name="endDate">Ending date range</param>
        /// <returns>The proportion (between 0 and 1) of tasks completed.</returns>
        [HttpGet("metrics/user/{emailAddress}/{startDate}/{endDate}"), Authorize]
        public async Task<IActionResult> GetUserCompletionRate(string emailAddress, DateTime startDate, DateTime endDate)
        {
            //Check if user exists
            var userExists = await unitOfWork.UserAccountRepository
                .UserAccountExists(emailAddress);
            //User doesn't exist
            if (!userExists) return NotFound("Couldn't find user.");
            //User exists
            double completionRate = await unitOfWork.CompletionLogEntryRepository.GetUserCompletionRate(emailAddress, startDate, endDate);
            return Ok(completionRate);
        }

        // GET api/tasks/metrics/team/{id}/{startDate}/{endDate}
        /// <summary>
        /// See how much of their tasks a team has completed.
        /// </summary>
        /// <param name="id">The key to identify the team.</param>
        /// <param name="startDate">Beginning date range</param>
        /// <param name="endDate">Ending date range</param>
        /// <returns>The proportion (between 0 and 1) of tasks completed.</returns>
        [HttpGet("metrics/team/{id}/{startDate}/{endDate}"), Authorize]
        public async Task<IActionResult> GetTeamCompletionRate(int id, DateTime startDate, DateTime endDate)
        {

            //Check if team exists
            //var teamExists = await context.Teams.AnyAsync(team => team.Id == id);
            var teamExists = await unitOfWork.UserTeamRepository
                .TeamExists(id);
            //Team doesn't exist
            if (!teamExists) return NotFound();
            //Team does exist
            double completionRate = await unitOfWork.CompletionLogEntryRepository.GetTeamCompletionRate(id, startDate, endDate);
            return Ok(completionRate);
        }
    }
}
