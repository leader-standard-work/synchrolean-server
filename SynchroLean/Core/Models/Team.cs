using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace SynchroLean.Core.Models
{
    public class Team
    {
        public int Id { get; set; }
        [Required]
        public int OwnerId { get; set; }
        [Required]
        [StringLength(25)]
        public string TeamName { get; set; }
        [StringLength(250)]
        public string TeamDescription { get; set; }
        public virtual ICollection<UserTask> AssociatedTasks { get; set; }
        public virtual ICollection<CompletionLogEntry> AssociatedLogEntries { get; set; }
        public virtual ICollection<Todo> AssociatedTodos { get; set; }
    }
}
