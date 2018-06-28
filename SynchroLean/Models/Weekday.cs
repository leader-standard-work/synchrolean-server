using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SynchroLean.Models
{
    public class Weekday
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<UserTask> Tasks { get; set; }

        public Weekday()
        {
            Tasks = new Collection<UserTask>();
        }
    }
}
