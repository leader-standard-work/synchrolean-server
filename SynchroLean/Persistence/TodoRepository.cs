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

        public async Task AddTodoAsync(Todo todo)
        {
            await context.Todos.AddAsync(todo);
        }

        public async Task<Todo> GetUsersTodo(int userId, int taskId)
        {
            return await context.Todos
                .Where(todo => todo.OwnerId == userId && todo.TaskId == taskId)
                .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Todo>> GetTodoListAsync(int ownerId)
        {
            return await context.Todos
                .Where(todo => todo.OwnerId.Equals(ownerId))
                .ToListAsync();
        }

        public async Task RemoveTodoTaskAsync(int todoId)
        {
            var todo = await context.Todos.SingleOrDefaultAsync(td => td.Id.Equals(todoId));
            if(todo == null){ return; }
            context.Remove(new Todo { Id = todoId});
        }

        public async Task CompleteTaskAsync(int todoId)
        {
            var todo = await context.Todos.FindAsync(todoId);
            if(todo == null){ return; }
            todo.IsCompleted = true;
            var task = await context.UserTasks.FindAsync(todo.TaskId);
            //For outstanding tasks without a due date, get it deleted before the end of the day
            if (!task.IsRecurring) todo.Expires = DateTime.Today.AddDays(1);
        }

        public async Task UndoCompleteTaskAsync(int todoId)
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