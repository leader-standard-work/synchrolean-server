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

        public async Task Forbid(int subjectId, int objectId)
        {
            var alreadyPermitted = await ((ITeamPermissionRepository)this).IsPermitted(subjectId, objectId);
            if(alreadyPermitted)
            {
                context.TeamPermissions.Remove(new TeamPermission{ SubjectTeamId = subjectId, ObjectTeamId = objectId});
            }
        }

        public async Task<IEnumerable<TeamPermission>> GetTeamPermissions()
        {
            return await context.TeamPermissions.ToListAsync();
        }

        public async Task<IEnumerable<Team>> GetTeamsThatCanSee(int objectId)
        {
            return await (
                from permissionRelation in context.TeamPermissions
                where permissionRelation.ObjectTeamId == objectId
                select permissionRelation.SubjectTeam
                ).ToListAsync();
        }

        public async Task<IEnumerable<Team>> GetTeamsThatItSees(int subjectId)
        {
            return await(
                from permissionRelation in context.TeamPermissions
                where permissionRelation.SubjectTeamId == subjectId
                select permissionRelation.ObjectTeam
                ).ToListAsync();
        }

        public async Task<ISet<int>> GetTeamIdsUserIdSees(int subjectId)
        {
            var teamsUserIsOn = await
                (from membership in context.TeamMembers
                 where membership.MemberId == subjectId
                 select membership.TeamId).ToListAsync();
            var teamsUserCanSee = await
               (from permissionRelation in context.TeamPermissions
                join teamId in teamsUserIsOn
                on permissionRelation.SubjectTeamId equals teamId
                select permissionRelation.ObjectTeamId).ToListAsync();
            return new HashSet<int>(teamsUserIsOn.Concat(teamsUserCanSee));
        }

        public async Task<bool> IsPermitted(int subjectId, int objectId)
        {
            return await context.TeamPermissions.AnyAsync(x => x.SubjectTeamId == subjectId && x.ObjectTeamId == objectId);
        }

        public async Task Permit(int subjectId, int objectId)
        {
            var alreadyPermitted = await((ITeamPermissionRepository)this).IsPermitted(subjectId, objectId);
            if (!alreadyPermitted)
            {
                context.TeamPermissions.Add(new TeamPermission { SubjectTeamId = subjectId, ObjectTeamId = objectId });
            }
        }

        public async Task<bool> UserIsPermittedToSeeTeam(int subjectUserId, int objectId)
        {
            var teamsUserIsIn =
                from membership in context.TeamMembers
                where membership.MemberId == subjectUserId
                select membership.TeamId;
            var result = false;
            result = await teamsUserIsIn.AnyAsync(x => x == objectId);
            if (result) return result;
            else
            {
                var teamsUserCanSee =
                    from teamId in teamsUserIsIn
                    join permission in context.TeamPermissions
                    on teamId equals permission.SubjectTeamId
                    select permission.ObjectTeamId;
                return await teamsUserCanSee.AnyAsync(x => x == objectId);
            }
        }

        public async Task<bool> UserIsPermittedToSeeUser(int subjectUserId, int objectUserId)
        {
            //Trivial case
            if (subjectUserId == objectUserId) return true;
            var possibleRelations =
                from subjectMembership in context.TeamMembers
                from objectMembership in context.TeamMembers
                where subjectMembership.MemberId == subjectUserId && objectMembership.MemberId == objectUserId
                select subjectMembership.MemberId == objectMembership.MemberId
                       || null != context.TeamPermissions.Find(subjectMembership.MemberId, objectMembership.MemberId);
            return await possibleRelations.AnyAsync(x => x);
        }
    }
}
