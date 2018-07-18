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

        public int MemberId { get; set; }

        [ForeignKey("TeamId")]
        public Team Team { get; set; }

        [ForeignKey("MemberId")]
        public UserAccount Member { get; set; }
    }
}
