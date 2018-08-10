using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SynchroLean.Core.Models
{
    public class Todo
    {
        public int TaskId { get; set; }
        public virtual UserTask Task { get; set; }
        public DateTime? Completed { get; set; }
        [Required]
        public DateTime Expires { get; set; }
        [NotMapped]
        public bool IsCompleted
        {
            get
            {
                return Completed != null;
            }
            set
            {
                //Only need to do something if the property is mismatched with the value
                if(value && !IsCompleted) Completed = DateTime.Now;
                else if(!value && IsCompleted) Completed = null;
            }
        }
        public static Todo FromTask(UserTask userTask, DateTime expires)
        {
            return new Todo
            {
                TaskId = userTask.Id,
                Expires = expires
            };
        }
    }
}