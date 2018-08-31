using System;
using SynchroLean.Core.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SynchroLean.Core;
using SynchroLean.Persistence;
using AutoMapper;

namespace SynchroLean.Controllers.Resources
{
    public class UserTaskResource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte Weekdays { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletionDate { get; set; }
        public bool IsDeleted { get; set; }
        public string OwnerEmail { get; set; }
        public Frequency Frequency { get; set; }
        public int? TeamId { get; set; }
        public bool IsActive { get; set; }
        /// <summary>
        /// Map a UserTask model to a UserTask resource with automapper
        /// </summary>
        /// <param name="mapper">The auto mapper to use</param>
        /// <param name="task">The task model to use. Its todo must be loaded if there is one</param>
        /// <returns></returns>
        public static UserTaskResource CreateWithMapper(IMapper mapper, UserTask task)
        {
            var mapped = mapper.Map<UserTaskResource>(task);
            var todo = task.Todo;
            mapped.IsCompleted = todo != null && todo.IsCompleted;
            mapped.CompletionDate = todo != null ? todo.Completed : null;
            mapped.IsActive = task.IsActive;
            return mapped;
        }
    }
}
