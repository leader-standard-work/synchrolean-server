using SynchroLean.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SynchroLean.Core
{
    interface IAddUserRequestRepository
    {
        Task AddAsync(AddUserRequest request);
        Task<AddUserRequest> GetAddUserRequestAsync(int addUserRequestId);
        Task<IEnumerable<AddUserRequest>> GetAddUserRequestsAsync();
        Task<IEnumerable<AddUserRequest>> GetAddUserRequestsPendingApprovalAsync(int ownerId);
        Task<IEnumerable<AddUserRequest>> GetAddUserRequestsPendingAcceptanceAsync(int ownerId);
        Task<Boolean> AddUserRequestExists(int addUserRequestId);
        Task DeleteAddUserRequestAsync(int addUserRequestId);
    }
}
