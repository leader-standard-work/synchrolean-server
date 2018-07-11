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
        AddUserRequestRespository(SynchroLeanDbContext context)
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

        Task IAddUserRequestRepository.DeleteAddUserRequestAsync(int addUserRequestId)
        {
            throw new NotImplementedException();
        }

        Task<AddUserRequest> IAddUserRequestRepository.GetAddUserRequestAsync(int addUserRequestId)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<AddUserRequest>> IAddUserRequestRepository.GetAddUserRequestsAsync()
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<AddUserRequest>> IAddUserRequestRepository.GetAddUserRequestsPendingAcceptanceAsync(int ownerId)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<AddUserRequest>> IAddUserRequestRepository.GetAddUserRequestsPendingApprovalAsync(int ownerId)
        {
            throw new NotImplementedException();
        }
    }
}
