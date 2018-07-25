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
        public int Id { get; set; }
        public int TaskId { get; set; }
        [ForeignKey("TaskId")]
        public UserTask Task { get; set; }
        public int OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        public UserAccount Owner { get; set; }
        public DateTime? Completed { get; set; }
        [Required]
        public DateTime Expires { get; set; }
    }
}