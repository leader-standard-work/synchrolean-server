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

        async Task<bool> IAddUserRequestRepository.AddUserRequestExists(int addUserRequestId)
        {
            return await this.context.AddUserRequests.AnyAsync(request => request.AddUserRequestId == addUserRequestId);
        }

        async Task IAddUserRequestRepository.DeleteAddUserRequestAsync(int addUserRequestId)
        {
            var toDelete = await ((IAddUserRequestRepository)this).GetAddUserRequestAsync(addUserRequestId);
            this.context.AddUserRequests.Remove(toDelete);
        }

        async Task<AddUserRequest> IAddUserRequestRepository.GetAddUserRequestAsync(int addUserRequestId)
        {
            return await
            (
                from request in this.context.AddUserRequests
                where request.AddUserRequestId == addUserRequestId
                select request
            ).SingleOrDefaultAsync();
        }

        async Task<IEnumerable<AddUserRequest>> IAddUserRequestRepository.GetAddUserRequestsAsync()
        {
            return await this.context.AddUserRequests.ToListAsync();
        }

        async Task<IEnumerable<AddUserRequest>> IAddUserRequestRepository.GetAddUserRequestsPendingAcceptanceAsync(int ownerId)
        {
            return await
            (
                from request in this.context.AddUserRequests
                where request.Invitee.OwnerId == ownerId && request.IsAuthorized
                select request
            ).ToListAsync();
        }

        async Task<IEnumerable<AddUserRequest>> IAddUserRequestRepository.GetAddUserRequestsPendingApprovalAsync(int ownerId)
        {
            return await
            (
                from request in this.context.AddUserRequests
                join ownedteam in
                (
                    from team in this.context.Teams
                    where team.OwnerId == ownerId
                    select team
                )
                on request.DestinationTeam equals ownedteam
                where !request.IsAuthorized
                select request
            ).ToListAsync();
        }
    }
}
