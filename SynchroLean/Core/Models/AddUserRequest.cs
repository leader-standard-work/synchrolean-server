using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SynchroLean.Core.Models
{
    public class AddUserRequest
    {
        /// <summary>
        /// The identity of this invitation to a team.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Which team the user will be added to.
        /// </summary>
        [Required]
        public int TeamId { get; set; }

        /// <summary>
        /// Who will join the team.
        /// </summary>
        [Required]
        public int OwnerId { get; set; }

        /// <summary>
        /// Who sent out the invitation.
        /// </summary>
        public int CreatorId { get; set; }

        /// <summary>
        /// Check if the owner of the group approved the invitation.
        /// </summary>
        [Required]
        public bool IsAuthorized { get; set; }
    }
}
