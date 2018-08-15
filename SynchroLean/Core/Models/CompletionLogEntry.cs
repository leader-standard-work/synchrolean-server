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
        public CompletionLogEntry(UserTask task, DateTime entryTime, bool completed)
        {
            this.TaskId = task.Id;
            this.OwnerEmail = task.OwnerEmail;
            this.EntryTime = entryTime;
            this.IsCompleted = completed;
            this.TeamId = task.TeamId;
        }
        public CompletionLogEntry(Todo todo)
        {
            var entryTime = todo.IsCompleted ? (DateTime)todo.Completed : todo.Expires;
            var task = todo.Task;
            this.TaskId = task.Id;
            this.OwnerEmail = task.OwnerEmail;
            this.EntryTime = entryTime;
            this.IsCompleted = todo.IsCompleted;
            this.TeamId = task.TeamId;
        }
    }
}