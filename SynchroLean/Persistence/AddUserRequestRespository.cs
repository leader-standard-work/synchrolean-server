using Microsoft.EntityFrameworkCore;
using SynchroLean.Core;
using SynchroLean.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SynchroLean.Persistence
{
    public class AddUserRequestRespository: IAddUserRequestRepository
    {
        SynchroLeanDbContext context;
        public AddUserRequestRespository(SynchroLeanDbContext context)
        {
            this.context = context;
        }

        async Task IAddUserRequestRepository.AddAsync(AddUserRequest request)
        {
            await this.context.AddUserRequests.AddAsync(request);
        }

        async Task<bool> IAddUserRequestRepository.AddUserRequestExists(string email, int teamId)
        {
            var invite = await this.context.AddUserRequests.FindAsync(email, teamId);
            return invite != null;
        }

        async Task IAddUserRequestRepository.DeleteAddUserRequestAsync(string email, int teamId)
        {
            var toDelete = await this.context.AddUserRequests.FindAsync(email, teamId);
            if(toDelete != null) this.context.AddUserRequests.Remove(toDelete);
        }

        async Task<AddUserRequest> IAddUserRequestRepository.GetAddUserRequestAsync(string email, int teamId)
        {
            return await this.context.AddUserRequests.FindAsync(email, teamId);
        }

        async Task<IEnumerable<AddUserRequest>> IAddUserRequestRepository.GetAddUserRequestsAsync()
        {
            return await this.context.AddUserRequests.ToListAsync();
        }

        async Task<IEnumerable<AddUserRequest>> IAddUserRequestRepository.GetAddUserRequestsPendingAcceptanceAsync(string emailAddress)
        {
            return await
            (
                from request in this.context.AddUserRequests
                where request.Invitee.Email == emailAddress && request.IsAuthorized
                select request
            ).ToListAsync();
        }

        async Task<IEnumerable<AddUserRequest>> IAddUserRequestRepository.GetAddUserRequestsPendingApprovalAsync(string emailAddress)
        {
            return await
            (
                from request in this.context.AddUserRequests
                join ownedteam in
                (
                    from team in this.context.Teams
                    where team.OwnerEmail == emailAddress
                    select team
                )
                on request.DestinationTeam equals ownedteam
                where !request.IsAuthorized
                select request
            ).ToListAsync();
        }

        async Task<IEnumerable<AddUserRequest>> IAddUserRequestRepository.GetMySentAddUserRequestsAsync(string emailAddress)
        {
            return await
            (
                from request in this.context.AddUserRequests
                where request.Inviter.Email == emailAddress
                select request
            ).ToListAsync();
        }
    }
}
