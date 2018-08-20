using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SynchroLean.Controllers.Resources
{
    public class WeeklyRollupResource
    {
        public int TeamId { get; set; }
        public IList<UserTaskResource> OutstandingTasks { get; set; }
        public IList<CompletionLogEntryResource> PastWeekTasks { get; set; }
        public double Completion { get; set; }
    }
}
