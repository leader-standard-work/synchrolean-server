using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace SynchroLean.Core.Models
{
    public class Team
    {
        public int Id { get; set; }
        [Required]
        public int OwnerId { get; set; }
        [Required]
        [StringLength(25)]
        public string TeamName { get; set; }
        [StringLength(250)]
        public string TeamDescription { get; set; }

        // Some kind of collection of team members here as well
    }
}
