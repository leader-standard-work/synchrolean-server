using System;
using SynchroLean.Core.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SynchroLean.Core;

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
        public DateTime? CompletionDate { get; set; }
        public bool IsDeleted { get; set; }
        public string OwnerEmail { get; set; }
        public Frequency Frequency { get; set; }
        public int? TeamId { get; set; }
    }
}