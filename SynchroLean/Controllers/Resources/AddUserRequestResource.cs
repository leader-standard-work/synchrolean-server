using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SynchroLean.Core.Models;

namespace SynchroLean.Controllers.Resources
{
    public class AddUserRequestResource
    {
        public string InviteeEmail { get; set; }
        public string InviterEmail { get; set; }
        public int TeamId { get; set; }
        public int InviteId { get; set; }
        public bool IsAuthorized { get; set; }
        public AddUserRequestResource(AddUserRequest invite)
        {
            this.InviteeEmail = invite.Invitee.Email;
            this.InviterEmail = invite.Inviter.Email;
            this.InviteId = invite.AddUserRequestId;
            this.TeamId = invite.DestinationTeam.Id;
            this.IsAuthorized = invite.IsAuthorized;
        }
    }
}
