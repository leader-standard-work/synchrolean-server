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

        public async Task AddTodoAsync(int taskId)
        {
            var task = await context.UserTasks.FindAsync(taskId);
            //Already in the list
            var alreadyExists = await context.Todos.AnyAsync(todo => todo.TaskId == taskId);
            //Doesn't apply to us
            var notToday = !task.OccursOnDayOfWeek(DateTime.Today.DayOfWeek);
            if(alreadyExists || notToday) return;
            //Otherwise, go ahead and add it
            var tomorrow = DateTime.Today + TimeSpan.FromDays(1);
            var endOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddDays(-1);
            var endOfWeek = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day)
                .AddDays((int)(DayOfWeek.Saturday) - (int)(DateTime.Now.DayOfWeek) + 1);
            var expiry =
                task.Frequency == Frequency.Monthly ? endOfMonth
                : task.Frequency == Frequency.Weekly ? endOfWeek
                : task.Frequency == Frequency.Daily ? tomorrow
                : DateTime.MaxValue;
            await context.Todos.AddAsync(Todo.FromTask(task, expiry));
        }

        public async Task<Todo> GetTodo(int taskId)
        {
            return await context.Todos.FindAsync(taskId);
        }

        public async Task<IEnumerable<Todo>> GetTodoListAsync(int ownerId)
        {
            return await context.Todos.Include(todo => todo.Task)
                .Where(todo => todo.Task.OwnerId.Equals(ownerId))
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
            var todo = await context.Todos.FindAsync(todoId);
            if(todo == null) return; 
            if(!todo.IsCompleted)
            {
                todo.IsCompleted = true;

                var task = await context.UserTasks.FindAsync(todo.TaskId);
                //For outstanding tasks without a due date, get it deleted before the end of the day
                if (!task.IsRecurring) todo.Expires = DateTime.Today.AddDays(1);

                //Create log entry and add to log
                var entry = new CompletionLogEntry {
                    TaskId = todo.TaskId,
                    EntryTime = DateTime.Now,
                    IsCompleted = todo.IsCompleted
                };
                await context.TaskCompletionLog.AddAsync(entry);
            }
        }

        public async Task UndoCompleteTodoAsync(int todoId)
        {
            var todo = await context.Todos.FindAsync(todoId);
            if (todo == null) return;
            // Check that todo is completed
            if(todo.IsCompleted)
            {
                var task = await context.UserTasks.FindAsync(todo.TaskId);
                //For outstanding tasks without a due date, make sure it isn't deleted at the end of the day
                if (!task.IsRecurring) todo.Expires = DateTime.MaxValue;

                //Find and remove log entry
                var entry = new CompletionLogEntry {
                    TaskId = todo.TaskId,
                    EntryTime = (DateTime)todo.Completed
                };

                context.TaskCompletionLog.Remove(entry);
                todo.IsCompleted = false;
            }
        }

        public async Task CleanTodos()
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
                context.TaskCompletionLog.Add(
                        new CompletionLogEntry
                        {
                            TaskId = expired.TaskId,
                            EntryTime = expired.Expires,
                            IsCompleted = false
                        }
                    );
                if (!expired.Task.IsRecurring) expired.Task.IsRemoved = true;
                context.Todos.Remove(expired);
            }
        }

        public async Task RefreshTodo(int taskId) 
        { 
            var task = context.UserTasks.Find(taskId); 
            if (task == null) return; //invalid, nothing to do 
            var todo = await context.Todos.Where(td => td.TaskId == taskId).FirstOrDefaultAsync(); 
            DateTime? todoCompletion = null; 
            if (todo != null) 
            { 
                todoCompletion = todo.Completed; 
                context.Todos.Remove(todo); 
            } 
            await this.AddTodoAsync(taskId); 
            var newTodo = await context.Todos.Where(td => td.TaskId == taskId).FirstOrDefaultAsync(); 
            if (newTodo != null) newTodo.Completed = todoCompletion; 
        } 
    }
}