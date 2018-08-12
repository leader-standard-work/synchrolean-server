using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SynchroLean.Core.Models
{
    /// <summary>
    /// This class contains fields for client identification information
    /// </summary>
    public class UserAccount
    {
        /// <value>Gets and sets user email</value>
        [Key]
        [Required]
        [StringLength(50)]
        public string Email { get; set; }
        /// <value>Gets and sets user first name</value>
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }
        /// <value>Gets and sets user last name</value>
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }
        /// <value>Gets and sets user password</value>
        [Required]
        public string Password { get; set; }
        /// <value>Gets and sets user salt</value>
        [Required]
        public string Salt { get; set; }
        public virtual ICollection<UserTask> Tasks { get; set; }
        public virtual ICollection<TeamMember> TeamMembershipRelations { get; set; }
        public virtual ICollection<AddUserRequest> OutgoingInvites { get; set; }
        public virtual ICollection<AddUserRequest> IncomingInvites { get; set; }
        [NotMapped]
        public IEnumerable<Todo> TodoList
        {
            get
            {
                return this.Tasks.Select(taskItem => taskItem.Todo);
            }
        }
        public DateTime? Deleted { get; set; }
        [NotMapped]
        public bool IsDeleted { get { return this.Deleted != null; } }
        public void Delete()
        {
            if(!this.IsDeleted) this.Deleted = DateTime.Now;
        }
    }
}
