using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SynchroLean.Core.Models;

namespace SynchroLean.Core {
    public interface ITodoRepository
    {
        /// <summary>
        /// Add a todo to the Db asynchronously
        /// </summary>
        /// <param name="todo"></param>
        /// <returns></returns>
        Task AddTodoAsync(Todo todo);

        /// <summary>
        /// Retrieve a todo from Db asynchronously
        /// </summary>
        /// <param name="todoId"></param>
        /// <returns></returns>
        Task<Todo> GetTodoAsync(int todoId);

        /// <summary>
        /// Remove a todo from Db
        /// </summary>
        /// <param name="todoId"></param>
        /// <returns></returns>
        Task RemoveTodoTaskAsync(int todoId);

        /// <summary>
        /// Mark todo as completed
        /// </summary>
        /// <param name="todoId"></param>
        /// <returns></returns>
        Task CompleteTaskAsync(int todoId);

        /// <summary>
        /// Mark all todos as missed and delete them, if they expired before or at the threshold
        /// </summary>
        /// <param name="threshold">The latest time for expiry.</param>
        /// <returns></returns>
        Task CleanTodos(DateTime threshold);
    }
}