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

        public async Task AddTodoTaskAsync(Todo todo)
        {
            await context.Todos.AddAsync(todo);
        }

        public async Task<Todo> GetTodoAsync(int taskid)
        {
            return await context.Todos
                .SingleOrDefaultAsync(td => td.TaskId.Equals(taskid));
        }

        public async Task RemoveTodoTaskAsync(int taskId)
        {
            var todo = await context.Todos.SingleOrDefaultAsync(td => td.Task.Equals(taskId));
            if(todo == null){ return; }
            context.Remove(new Todo { TaskId = taskId});
        }

        public async Task CompleteTaskAsync(int taskId)
        {
            var todo = await context.Todos.SingleOrDefaultAsync(td => td.TaskId.Equals(taskId));
            if(todo == null){ return; }
            todo.Completed = DateTime.Now;
        }

        public async Task TodoMissAsync(int taskId)
        {
            var todo = await context.Todos.SingleOrDefaultAsync(td => td.TaskId.Equals(taskId));
            if(todo == null){ return; }
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