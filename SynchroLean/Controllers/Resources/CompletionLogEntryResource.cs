using System;

namespace SynchroLean.Controllers.Resources
{
    public class CompletionLogEntryResource
    {
        public int TaskId { get; set; }
        public string OwnerEmail { get; set; }
        public DateTime EntryTime { get; set; }
        public bool IsCompleted { get; set; }
        public int TeamId { get; set; }
    }
}