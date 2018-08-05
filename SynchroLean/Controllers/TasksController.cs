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

        // POST api/tasks
        /// <summary>
        /// Adds new task to DB
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="userTaskResource"></param>
        /// <returns>New task retrieved from DB</returns>
        [HttpPost]
        public async Task<IActionResult> AddUserTaskAsync([FromBody]UserTaskResource userTaskResource)
        {
            // How does this validate against the UserTask model?
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Map object from UserTaskResource into UserTask
            var userTask = _mapper.Map<UserTask>(userTaskResource);

            // Save userTask to database
            await unitOfWork.userTaskRepository.AddAsync(userTask);
            await unitOfWork.CompleteAsync();
            
            // If the task is valid now, immediately push it to the todo list
            if(!userTask.IsRecurring 
                || !(userTask.Frequency == Frequency.Daily) 
                || userTask.OccursOnDayOfWeek(DateTime.Today.DayOfWeek))
            {
                await unitOfWork.todoList.AddTodoAsync(userTask.Id);
            }

            // Retrieve userTask from database
            userTask = await unitOfWork.userTaskRepository
                .GetTaskAsync(userTask.Id);

            // Return mapped resource
            return Ok(_mapper.Map<UserTaskResource>(userTask));
        }

        // GET api/tasks/{ownerId}
        /// <summary>
        /// Retrieves a users tasks
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns>List of a users tasks</returns>
        [HttpGet("{ownerId}")]
        public async Task<IActionResult> GetTasksAsync(int ownerId)
        {
            // Fetch all tasks from the DB asyncronously
            var tasks = await unitOfWork.userTaskRepository
                .GetTasksAsync(ownerId);

            // List of corresponding tasks as resources
            var resourceTasks = new List<UserTaskResource>();

            // Map each task to a corresponding resource
            foreach (var task in tasks)
            {
                // Add mapped resource to resources list
                if(!task.IsRemoved){
                    resourceTasks.Add(_mapper.Map<UserTaskResource>(task));
                }
            }
                
            return Ok(resourceTasks); // List of UserTaskResources 200OK
        }

        /// <summary>
        /// Retrieves the users tasks for current day
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns>A list of current days tasks</returns>
        [HttpGet("todo/{ownerId}")]
        public async Task<IActionResult> GetTodosAsync(int ownerId)
        {
            // Check that account exists (can be removed with auth??)
            var account = await unitOfWork.userAccountRepository
                .GetUserAccountAsync(ownerId);

            if(account == null)
            {
                return NotFound("Account was not found!");
            }

            // Get todos from Db asynchronously
            var todos = await unitOfWork.todoList.GetTodoListAsync(ownerId);

            // Count check might be unnecessary 
            if(todos == null || todos.Count() == 0)
            {
                return NotFound("No Task found for today");
            }

            // Return current days tasks
            /*var taskResources = todos.Select(todo => _mapper
                .Map<UserTaskResource>(unitOfWork.userTaskRepository
                .GetTaskAsync(todo.TaskId).Result));
            */
            var taskResources = new List<UserTaskResource>();

            foreach(var todo in todos)
            {
                var taskResource = _mapper
                    .Map<UserTaskResource>(unitOfWork.userTaskRepository
                    .GetTaskAsync(todo.TaskId).Result);
                taskResource.IsCompleted = todo.IsCompleted;
                if(todo.IsCompleted)
                    taskResource.CompletionDate = (DateTime)todo.Completed;

                taskResources.Add(taskResource);
            }
            
            return Ok(taskResources);
        }

        /// <summary>
        /// Gets a single task from user task
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="taskId"></param>
        /// <returns>A task specified by taskId</returns>
        [HttpGet("{ownerId}/{taskId}")]
        public async Task<IActionResult> GetTaskAsync(int ownerId, int taskId)
        {
            // Check that account exists
            if(await unitOfWork.userAccountRepository.GetUserAccountAsync(ownerId) == null)
            {
                return NotFound("Account not found!");
            }

            // Retrieve task
            var task = await unitOfWork.userTaskRepository.GetTaskAsync(taskId);

            // Check if task exists
            if(task == null || task.IsRemoved)
            {
                return NotFound("Task not found!");
            } else 
            {
                return Ok(_mapper.Map<UserTaskResource>(task));
            }
        }

        // PUT api/tasks/{ownerId}/{taskId}
        /// <summary>
        /// Updates a users task
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="taskId"></param>
        /// <param name="userTaskResource"></param>
        /// <returns>Updated user task</returns>
        [HttpPut("{ownerId}/{taskId}")]
        public async Task<IActionResult> EditUserTaskAsync(int ownerId, int taskId, [FromBody]UserTaskResource userTaskResource)
        {
            // How does this validate against the UserTask model?
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // Fetch an account from the DB asynchronously
            var account = await unitOfWork.userAccountRepository
                .GetUserAccountAsync(ownerId);

            // Return not found exception if account doesn't exist
            if(account == null)
            {
                return NotFound("Couldn't find account matching that ownerId.");
            }

            // Retrieves task from UserTasks table
            var task = await unitOfWork.userTaskRepository
                .GetTaskAsync(taskId);

            // Nothing was retrieved, no id match
            if (task == null || task.IsRemoved)
            {
                return NotFound("Task couldn't be found.");
            }

            // Validates task belongs to correct user
            if(task.OwnerId != account.OwnerId)
            {
                return BadRequest("Task does not belong to this account.");
            }

            // Don't know if it's possible to AutoMap without creating a new model
            // This doesn't work but I'm trying to do something along this line
            //task = _mapper.Map<UserTask>(userTaskResource);

            //Check if a todo for that task exists
            var todo = await unitOfWork.todoList.GetUserTodo(ownerId,taskId);
            var todoExists = !(todo == null);
            //Complete the task if needed
            if(todoExists)
            {
                if (userTaskResource.IsCompleted && !todo.IsCompleted)
                    await unitOfWork.todoList.CompleteTodoAsync(todo.Id);
                else if (!userTaskResource.IsCompleted && todo.IsCompleted)
                    await unitOfWork.todoList.UndoCompleteTodoAsync(todo.Id);
            }
            //Delete the task if needed
            //Remove the todo if needed
            if(userTaskResource.IsRemoved)
            {
                await unitOfWork.todoList.RemoveTodosAsync(taskId);
            }

            // Map resource to model
            task.Name = userTaskResource.Name;
            task.Description = userTaskResource.Description;
            task.IsRecurring = userTaskResource.IsRecurring;
            task.Weekdays = userTaskResource.Weekdays;
            task.IsRemoved = userTaskResource.IsRemoved;
            task.OwnerId = userTaskResource.OwnerId;
            // Save updated userTask to database
            await unitOfWork.CompleteAsync();

            // Return mapped resource
            return Ok(_mapper.Map<UserTaskResource>(task));
        }

        // GET api/tasks/metrics/user/{ownerId}
        /// <summary>
        /// Get the completion rate for a user.
        /// </summary>
        /// <param name="ownerId">The key to identify the owner.</param>
        /// <returns>The proportion (between 0 and 1) of tasks completed.</returns>
        [HttpGet("metrics/user/{ownerId}/{startDate}/{endDate}")]
        public async Task<IActionResult> GetUserCompletionRate(int ownerId, DateTime startDate, DateTime endDate)
        {
            //Check if user exists
            var userExists = await unitOfWork.userAccountRepository
                .UserAccountExists(ownerId);
            //User doesn't exist
            if (!userExists) return NotFound("Couldn't find user.");
            //User exists
            Double completionRate = await unitOfWork.completionLogEntryRepository.GetUserCompletionRate(ownerId, startDate, endDate);
            return Ok(completionRate);
        }

        /// <summary>
        /// See how much of their tasks a team has completed.
        /// </summary>
        /// <param name="id">The key to identify the team.</param>
        /// <returns>The proportion (between 0 and 1) of tasks completed.</returns>
        [HttpGet("metrics/team/{id}/{startDate}/{endDate}")]
        public async Task<IActionResult> GetTeamCompletionRate(int id, DateTime startDate, DateTime endDate)
        {

            //Check if team exists
            //var teamExists = await context.Teams.AnyAsync(team => team.Id == id);
            var teamExists = await unitOfWork.userTeamRepository
                .TeamExists(id);
            //Team doesn't exist
            if (!teamExists) return NotFound();
            //Team does exist
            Double completionRate = await unitOfWork.completionLogEntryRepository.GetTeamCompletionRate(id, startDate, endDate);
            return Ok(completionRate);
        }
    }
}
