using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SynchroLean.Core;
using SynchroLean.Core.Models;
using SynchroLean.Persistence;

namespace SynchroLean.Persistence 
{
    public class TodoRepository : ITodoRepository
    {
        private readonly SynchroLeanDbContext context;

        public TodoRepository(SynchroLeanDbContext context)
        {
            this.context = context;
        }

        /*public async Task AddTodoAsync(int taskId)
        {
            var task = await context.UserTasks.Include(ut => ut.Owner).FirstOrDefaultAsync(ut => ut.Id.Equals(taskId));
            if (task == null) return; //invalid task, nothing to do
            //Task is deleted
            if (task.IsDeleted) return; //task is deleted, nothing to do
            //Already in the list
            var alreadyExists = await context.Todos.AnyAsync(todo => todo.TaskId == taskId);
            if (alreadyExists) return; //abort
            //Doesn't apply to us
            var notToday = !task.OccursOnDayOfWeek(DateTime.Today.DayOfWeek);
            if (notToday) return;
            //Team that it is assigned to was deleted
            var team = task.Team;
            var teamDeleted = team != null && team.IsDeleted;
            if (teamDeleted) return;
            //Check that user is in the team
            var user = task.Owner;
            var ownerNotInTeam = team != null && !(team.Members.Contains(user));
            if(ownerNotInTeam) return;
            //Otherwise, go ahead and add it
            var tomorrow = DateTime.Today + TimeSpan.FromDays(1);
            var endOfWeek = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day)
                .AddDays((int)(DayOfWeek.Saturday) - (int)(DateTime.Now.DayOfWeek) + 1);
            var endOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddDays(-1);
            var expiry =
                task.Frequency == Frequency.Monthly ? endOfMonth
                : task.Frequency == Frequency.Weekly ? endOfWeek
                : task.Frequency == Frequency.Daily ? tomorrow
                : DateTime.MaxValue;
            await context.Todos.AddAsync(Todo.FromTask(task, expiry));
        }
        */

        public async Task AddTodoAsync(int taskId)
        {
            var task = await context.UserTasks.Include(ut => ut.Owner).FirstOrDefaultAsync(ut => ut.Id.Equals(taskId));
            if (task == null) return; //invalid task, nothing to do
            //Task is deleted
            if (task.IsDeleted) return; //task is deleted, nothing to do
            //Already in the list
            var alreadyExists = await context.Todos.AnyAsync(todo => todo.TaskId == taskId);
            if (alreadyExists) return; //abort
            //Doesn't apply to us
            var notToday = !task.OccursToday(DateTime.Today.DayOfWeek);
            if (notToday) return;
            //Team that it is assigned to was deleted
            var team = task.Team;
            var teamDeleted = team != null && team.IsDeleted;
            if (teamDeleted) return;
            //Check that user is in the team
            var user = task.Owner;
            var ownerNotInTeam = team != null && !(team.Members.Contains(user));
            if(ownerNotInTeam) return;
            //Otherwise, go ahead and add it
            //For daily tasks
            var tomorrow = DateTime.Today + TimeSpan.FromDays(1);
            //For all other tasks
            DateTime? dueDate = null;
            if(task.Frequency != Frequency.Daily) dueDate = task.DueDate;
            
            var expiry = 
                task.Frequency == Frequency.Daily ? tomorrow : (DateTime) dueDate.Value.AddDays(1);
            await context.Todos.AddAsync(Todo.FromTask(task, expiry));
        }

        public async Task<Todo> GetTodo(int taskId)
        {
            return await context.Todos.Include(todo => todo.Task).FirstOrDefaultAsync(todo => todo.TaskId == taskId);
        }

        public async Task<IEnumerable<Todo>> GetTodoListAsync(string emailAddress)
        {
            return await context.Todos.Include(todo => todo.Task)
                .Where(todo => todo.Task.OwnerEmail.Equals(emailAddress))
                .ToListAsync();
        }

        public void RemoveTodo(int taskId)
        {
            context.Remove(new Todo { TaskId = taskId});
        }

        public async Task RemoveTodosAsync(int taskId)
        {
            var todosForRemoval = await
                (from todo in context.Todos
                 where todo.TaskId == taskId
                 select todo).ToListAsync();
            foreach (Todo todo in todosForRemoval) context.Todos.Remove(todo);
        }

        public async Task CompleteTodoAsync(int todoId)
        {
            var todo = await context.Todos.Include(td => td.Task).FirstOrDefaultAsync(td => td.TaskId == todoId);
            if(todo == null) return; 
            if(!todo.IsCompleted)
            {
                todo.IsCompleted = true;
                
                //For outstanding tasks without a due date, get it deleted before the end of the day
                if (!todo.Task.IsRecurring) todo.Expires = DateTime.Today.AddDays(1);

                //Create log entry and add to log
                var entry = CompletionLogEntry.FromTodo(todo);
                await context.TaskCompletionLog.AddAsync(entry);
            }
        }

        public async Task UndoCompleteTodoAsync(int todoId)
        {
            var todo = await context.Todos.Include(td => td.Task).FirstOrDefaultAsync(td => td.TaskId == todoId);
            if (todo == null) return;
            // Check that todo is completed
            if(todo.IsCompleted)
            {//Find and remove log entry
                var entry = CompletionLogEntry.FromTodo(todo);
                context.TaskCompletionLog.Remove(entry);

                //For outstanding tasks without a due date, make sure it isn't deleted at the end of the day
                if (!todo.Task.IsRecurring) todo.Expires = DateTime.MaxValue;
                todo.IsCompleted = false;
            }
        }

        public async Task Clean()
        {
            DateTime threshold = DateTime.Now;
            var expireds = await 
                (
                    from todo in context.Todos.Include(todo => todo.Task)
                    where todo.Expires <= threshold
                    select todo
                ).ToListAsync();
            foreach(var expired in expireds)
            {
                context.TaskCompletionLog.Add(CompletionLogEntry.FromTodo(expired));
                if (!expired.Task.IsRecurring) expired.Task.Delete();
                context.Todos.Remove(expired);
                if (expired.Task.Frequency == Frequency.Weekly)
                {
                    expired.Task.DueDate.Value.AddDays(7);
                } else if (expired.Task.Frequency == Frequency.Monthly)
                {
                    expired.Task.DueDate.Value.AddMonths(1);
                }
            }
        }

        public async Task RefreshTodo(int taskId) 
        { 
            var task = context.UserTasks.Find(taskId); 
            if (task == null) return; //invalid, nothing to do 
            var todo = context.Todos.Find(taskId); 
            DateTime? todoCompletion = null; 
            if (todo != null) 
            { 
                todoCompletion = todo.Completed; 
                context.Todos.Remove(todo);
                await context.SaveChangesAsync();
            }
            await this.AddTodoAsync(taskId); 
            var newTodo = await context.Todos.FindAsync(taskId); 
            if (newTodo != null) newTodo.Completed = todoCompletion; 
        } 
    }
}
