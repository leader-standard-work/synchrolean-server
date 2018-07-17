using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SynchroLean.Core.Models;

namespace SynchroLean.Controllers.Resources
{
    public class AddUserRequestResource
    {
        public int InviteeId { get; set; }
        public int InviterId { get; set; }
        public int TeamId { get; set; }
        public int InviteId { get; set; }
        public bool IsAuthorized { get; set; }
        public AddUserRequestResource(AddUserRequest invite)
        {
            this.InviteeId = invite.Invitee.OwnerId;
            this.InviterId = invite.Inviter.OwnerId;
            this.InviteId = invite.AddUserRequestId;
            this.TeamId = invite.DestinationTeam.Id;
            this.IsAuthorized = invite.IsAuthorized;
        }
    }
}
