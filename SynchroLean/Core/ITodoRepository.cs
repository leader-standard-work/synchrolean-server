using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SynchroLean.Core.Models;

namespace SynchroLean.Core {
    public interface ITodoRepository
    {
        // add
        Task AddTodoTaskAsync(Todo todo);

        // get todo
        Task<Todo> GetTodoAsync(int taskId);

        // remove
        Task RemoveTodoTaskAsync(int taskId);

        // completed
        Task CompleteTaskAsync(int taskId);

        // inverse of complete??


        // miss
        Task TodoMissAsync(int taskId);

        /// <summary>
        /// Mark all todos as missed and delete them, if they expired before or at the threshold
        /// </summary>
        /// <param name="threshold">The latest time for expiry.</param>
        /// <returns></returns>
        Task CleanTodos(DateTime threshold);
    }
}