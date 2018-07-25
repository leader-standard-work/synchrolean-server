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
        public async Task AddTodoTaskAsync(int ownerId, DateTime expires, UserTask userTask)
        {
            Todo todoTask = new Todo {
                TaskId = userTask.Id,
                Task = userTask,
                OwnerId = ownerId,
                Owner = await context.UserAccounts
                        .Where(ua => ua.OwnerId.Equals(userTask.Id))
                        .SingleOrDefaultAsync(),
                Completed = null,
                Expires = expires
            };

            await context.Todos.AddAsync(todoTask);
        }

        public Task<bool> CompleteTask(int ownerId, int taskId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Todo>> EditTodoTask(int ownerId, int taskId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Todo>> RemoveTodoTask(int ownerId, int taskId)
        {
            throw new NotImplementedException();
        }
    }
}