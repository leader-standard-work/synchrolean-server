using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SynchroLean.Controllers.Resources
{
    public class TeamResource
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string TeamName { get; set; }
        public string TeamDescription { get; set; }

        // Some kind of collection of team members here as well
    }
}
