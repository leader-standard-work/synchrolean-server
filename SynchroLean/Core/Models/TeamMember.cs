using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SynchroLean.Core.Models
{
    public class TeamMember
    {
        public int TeamId { get; set; }

        public string MemberEmail { get; set; }

        [ForeignKey("TeamId")]
        public virtual Team Team { get; set; }

        [ForeignKey("MemberEmail")]
        public virtual UserAccount Member { get; set; }
    }
}
