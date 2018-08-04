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
            var notToday = (task.IsRecurring
                            && task.Frequency == Frequency.Daily
                            && !task.OccursOnDayOfWeek(DateTime.Today.DayOfWeek));
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

        public async Task<Todo> GetUserTodo(int userId, int todoId)
        {
            return await context.Todos
                .Where(todo => todo.OwnerId == userId && todo.TaskId == todoId)
                .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Todo>> GetTodoListAsync(int ownerId)
        {
            return await context.Todos
                .Where(todo => todo.OwnerId.Equals(ownerId))
                .ToListAsync();
        }

        public void RemoveTodo(int todoId)
        {
            context.Remove(new Todo { Id = todoId});
        }

        public async Task RemoveTodosAsync(int todoId)
        {
            var todosForRemoval = await
                (from todo in context.Todos
                 where todo.TaskId == todoId
                 select todo).ToListAsync();
            foreach (Todo todo in todosForRemoval) context.Todos.Remove(todo);
        }

        public async Task CompleteTodoAsync(int todoId)
        {
            var todo = await context.Todos.FindAsync(todoId);
            if(todo == null){ return; }
            todo.IsCompleted = true;
            var task = await context.UserTasks.FindAsync(todo.TaskId);
            //For outstanding tasks without a due date, get it deleted before the end of the day
            if (!task.IsRecurring) todo.Expires = DateTime.Today.AddDays(1);
        }

        public async Task UndoCompleteTodoAsync(int todoId)
        {
            var todo = await context.Todos.FindAsync(todoId);
            if (todo == null) return;
            todo.IsCompleted = false;
            var task = await context.UserTasks.FindAsync(todo.TaskId);
            //For outstanding tasks without a due date, make sure it isn't deleted at the end of the day
            if (!task.IsRecurring) todo.Expires = DateTime.MaxValue;
        }

        public async Task CleanTodos(DateTime threshold)
        {
            var expireds = await 
                (
                    from todo in context.Todos
                    where todo.Expires <= threshold
                    select todo
                ).ToListAsync();
            foreach(var expired in expireds)
            {
                context.TaskCompletionLog.Add(
                        new CompletionLogEntry
                        {
                            TaskId = expired.TaskId,
                            OwnerId = expired.OwnerId,
                            EntryTime = threshold,
                            IsCompleted = false
                        }
                    );
                context.Todos.Remove(expired);
            }
        }
    }
}