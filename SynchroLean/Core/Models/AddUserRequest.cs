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
        public int AddUserRequestId { get; set; }

        /// <summary>
        /// Which team the user will be added to.
        /// </summary>
        [Required]
        public virtual Team DestinationTeam { get; set; }

        /// <summary>
        /// Who will join the team.
        /// </summary>
        [Required]
        public virtual UserAccount Invitee { get; set; }

        /// <summary>
        /// Who sent out the invitation.
        /// </summary>
        public virtual UserAccount Inviter { get; set; }

        /// <summary>
        /// Check if the owner of the group approved the invitation.
        /// </summary>
        [Required]
        public bool IsAuthorized { get; set; }
    }
}
