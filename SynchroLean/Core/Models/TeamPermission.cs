using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SynchroLean.Core.Models
{
    public class TeamPermission
    {
        [Required]
        public int SubjectTeamId { get; set; }
        
        [Required]
        public int ObjectTeamId { get; set; }

        [ForeignKey("SubjectTeamId")]
        public virtual Team SubjectTeam { get; set; }

        [ForeignKey("ObjectTeamId")]
        public virtual Team ObjectTeam { get; set; }
    }
}
