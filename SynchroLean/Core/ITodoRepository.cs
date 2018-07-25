using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SynchroLean.Core.Models;

namespace SynchroLean.Core {
    public interface ITodoRepository
    {
        // add
        Task AddTodoTaskAsync(int ownerId, DateTime expires, UserTask UserTask);

        // update
        Task<IEnumerable<Todo>> EditTodoTask(int ownerId, int taskId);

        // remove
        Task<IEnumerable<Todo>> RemoveTodoTask(int ownerId, int taskId);

        // completed
        Task<bool> CompleteTask(int ownerId, int taskId);

        // inverse of complete??


        // miss

    }
}