using System;
using System.ComponentModel.DataAnnotations;

namespace SynchroLean.Core.Models
{
    /// <summary>
    /// This class contains fields for client identification information
    /// </summary>
    public class UserAccount
    {
        [Key]
        /// <value>Gets and sets user id number</value>
        public int OwnerId { get; set; }
        /// <value>Gets and sets team id number of user</value>
        public int TeamId { get; set; }
        /// <value>Gets and sets user first name</value>
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }
        /// <value>Gets and sets user last name</value>
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }
        /// <value>Gets and sets user email</value>
        [Required]
        [StringLength(50)]
        public string Email { get; set; }
        /// <value>Gets and sets account active/inactive state</value>
        public bool IsDeleted { get; set; }
    }
}