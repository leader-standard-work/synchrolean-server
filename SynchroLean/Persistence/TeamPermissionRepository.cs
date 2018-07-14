using Microsoft.EntityFrameworkCore;
using SynchroLean.Core;
using SynchroLean.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SynchroLean.Persistence
{
    public class TeamPermissionRepository : ITeamPermissionRepository
    {
        private SynchroLeanDbContext context;

        public TeamPermissionRepository(SynchroLeanDbContext context)
        {
            this.context = context;
        }

        async Task ITeamPermissionRepository.Forbid(int subjectId, int objectId)
        {
            var alreadyPermitted = await ((ITeamPermissionRepository)this).IsPermitted(subjectId, objectId);
            if(alreadyPermitted)
            {
                context.TeamPermissions.Remove(new TeamPermission{ SubjectTeamId = subjectId, ObjectTeamId = objectId});
            }
        }

        async Task<IEnumerable<TeamPermission>> ITeamPermissionRepository.GetTeamPermissions()
        {
            return await context.TeamPermissions.ToListAsync();
        }

        async Task<IEnumerable<int>> ITeamPermissionRepository.GetTeamsThatCanSee(int objectId)
        {
            return await (
                from permissionRelation in context.TeamPermissions
                where permissionRelation.ObjectTeamId == objectId
                select permissionRelation.SubjectTeamId
                ).ToListAsync();
        }

        async Task<IEnumerable<int>> ITeamPermissionRepository.GetTeamsThatItSees(int subjectId)
        {
            return await(
                from permissionRelation in context.TeamPermissions
                where permissionRelation.SubjectTeamId == subjectId
                select permissionRelation.ObjectTeamId
                ).ToListAsync();
        }

        async Task<bool> ITeamPermissionRepository.IsPermitted(int subjectId, int objectId)
        {
            return await context.TeamPermissions.AnyAsync(x => x.SubjectTeamId == subjectId && x.ObjectTeamId == objectId);
        }

        async Task ITeamPermissionRepository.Permit(int subjectId, int objectId)
        {
            var alreadyPermitted = await((ITeamPermissionRepository)this).IsPermitted(subjectId, objectId);
            if (!alreadyPermitted)
            {
                context.TeamPermissions.Add(new TeamPermission { SubjectTeamId = subjectId, ObjectTeamId = objectId });
            }
        }
    }
}
