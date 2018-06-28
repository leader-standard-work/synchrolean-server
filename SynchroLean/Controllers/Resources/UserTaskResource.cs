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
    }
}
