using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SynchroLean.Core.Models;

namespace SynchroLean.Core {
    public interface ITodoRepository
    {
        /// <summary>
        /// Add a new todo item for a task matching the task ID. If the task already
        ///  exists or doesn't apply to today, it is not added.
        /// </summary>
        /// <param name="taskId">The ID of the task to add.</param>
        /// <returns></returns>
        Task AddTodoAsync(int taskId);

        /// <summary>
        /// Get a specific user's todo for a task.
        /// </summary>
        /// <param name="userId">The id for the user.</param>
        /// <param name="taskId">The id for the task.</param>
        /// <returns>The todo item for the task. Null if the task wasn't assigned today.</returns>
        Task<Todo> GetUserTodo(int userId, int todoId);

        /// <summary>
        /// Retrieve a todo from Db asynchronously
        /// </summary>
        /// <param name="todoId"></param>
        /// <returns></returns>
        Task<IEnumerable<Todo>> GetTodoListAsync(int ownerId);

        /// <summary>
        /// Remove a todo from Db
        /// </summary>
        /// <param name="todoId"></param>
        /// <returns></returns>
        void RemoveTodo(int todoId);

        /// <summary>
        /// Remove all todo items associated with a particular task
        /// </summary>
        /// <param name="taskId">The id of that task</param>
        /// <returns></returns>
        Task RemoveTodosAsync(int todoId);

        /// <summary>
        /// Mark todo as completed
        /// </summary>
        /// <param name="todoId"></param>
        /// <returns></returns>
        Task CompleteTodoAsync(int todoId);

        /// <summary>
        /// Mark a completed todo as no longer complete
        /// </summary>
        /// <param name="todoId">The id of the todo to undo completion for.</param>
        /// <returns></returns>
        Task UndoCompleteTodoAsync(int todoId);

        /// <summary>
        /// Mark all todos as missed and delete them, if they expired before or at the threshold
        /// </summary>
        /// <param name="threshold">The latest time for expiry.</param>
        /// <returns></returns>
        Task CleanTodos(DateTime threshold);

    }
}