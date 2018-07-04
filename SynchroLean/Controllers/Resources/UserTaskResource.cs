using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SynchroLean.Controllers.Resources
{
    public class UserTaskResource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsRecurring { get; set; }
        public byte Weekdays { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CompletionDate { get; set; }
        public bool IsRemoved { get; set; }
        public int OwnerId { get; set; }
    }
}
