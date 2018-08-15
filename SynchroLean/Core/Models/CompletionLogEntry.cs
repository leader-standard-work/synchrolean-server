using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SynchroLean.Core.Models
{
    public class CompletionLogEntry
    {
        public int TaskId { get; set; }
        [ForeignKey("TaskId")]
        public virtual UserTask Task { get; set; }
        public string OwnerEmail { get; set; }
        [ForeignKey("OwnerEmail")]
        public virtual UserAccount Owner { get; set; }
        public DateTime EntryTime { get; set; }
        public bool IsCompleted { get; set; }
        public int? TeamId { get; set; }
        [ForeignKey("TeamId")]
        public virtual Team Team { get; set; }
        public static CompletionLogEntry FromTask(UserTask task, DateTime entryTime, bool completed)
        {
            return new CompletionLogEntry
            {
                TaskId = task.Id,
                OwnerEmail = task.OwnerEmail,
                EntryTime = entryTime,
                IsCompleted = completed,
                TeamId = task.TeamId
            };
        }
        public static CompletionLogEntry FromTodo(Todo todo)
        {
            var entryTime = todo.IsCompleted ? (DateTime)todo.Completed : todo.Expires;
            var task = todo.Task;
            return new CompletionLogEntry
            {
                TaskId = task.Id,
                OwnerEmail = task.OwnerEmail,
                EntryTime = entryTime,
                IsCompleted = todo.IsCompleted,
                TeamId = task.TeamId
            };
        }
    }
}