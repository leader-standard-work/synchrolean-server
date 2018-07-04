using System;
using System.ComponentModel.DataAnnotations;

namespace SynchroLean.Models
{
    public class UserAccount
    {
        public int OwnerId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [StringLength(50)]
        public string Email { get; set; }
    }
}