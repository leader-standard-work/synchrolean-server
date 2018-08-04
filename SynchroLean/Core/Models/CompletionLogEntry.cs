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
        public int OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        public virtual UserAccount Owner { get; set; }
        public DateTime EntryTime { get; set; }
        public bool IsCompleted { get; set; }
    }
}