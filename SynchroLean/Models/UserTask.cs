using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace SynchroLean.Models
{
    public class UserTask
    {
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsRecurring { get; set; }
        public ICollection<Weekday> Weekdays { get; set; }

        public UserTask()
        {
            Weekdays = new Collection<Weekday>();
        }
    }
}
