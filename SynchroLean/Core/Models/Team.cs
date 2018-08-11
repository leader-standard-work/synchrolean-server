using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SynchroLean.Core.Models
{
    public class Team
    {
        public int Id { get; set; }
        [Required]
        public string OwnerEmail { get; set; }
        [Required]
        [StringLength(25)]
        public string TeamName { get; set; }
        [StringLength(250)]
        public string TeamDescription { get; set; }
        public virtual ICollection<UserTask> AssociatedTasks { get; set; }
        public virtual ICollection<CompletionLogEntry> AssociatedLogEntries { get; set; }
        public virtual ICollection<TeamPermission> PermissionsWhereThisIsSubject { get; set; }
        public virtual ICollection<TeamPermission> PermissionsWhereThisIsObject { get; set; }
        [NotMapped]
        public virtual IEnumerable<Team> TeamsThatCanSeeThis
        {
            get
            {
                return PermissionsWhereThisIsObject.Select(perm => perm.SubjectTeam);
            }
        }

        [NotMapped]
        public virtual IEnumerable<Team> TeamsThatThisCanSee
        {
            get
            {
                return PermissionsWhereThisIsSubject.Select(perm => perm.ObjectTeam);
            }
        }

        public virtual ICollection<TeamMember> TeamMembershipRelations { get; set; }

        [NotMapped]
        public IEnumerable<UserAccount> Members
        {
            get
            {
                return this.TeamMembershipRelations.Select(relation => relation.Member);
            }
        }

        public DateTime? Deleted { get; set; }
        [NotMapped]
        public bool IsDeleted { get { return this.Deleted != null; } }
        public void Delete()
        {
            if (!this.IsDeleted)
            {
                this.Deleted = DateTime.Now;
                this.AssociatedTasks.Clear();
            }
        }

        public virtual ICollection<AddUserRequest> Invites { get; set; }

    }
}
