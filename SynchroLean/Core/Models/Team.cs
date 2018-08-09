using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public DateTime? Deleted { get; set; }
        [NotMapped]
        public bool IsDeleted { get { return this.Deleted != null; } }
        public void Delete()
        {
            this.Deleted = DateTime.Now;
            this.AssociatedTasks.Clear();
            this.AssociatedTodos.Clear();
        }
    }
}
