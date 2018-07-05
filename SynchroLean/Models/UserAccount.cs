using System;
using System.ComponentModel.DataAnnotations;

namespace SynchroLean.Models
{
    public class UserAccount
    {
        [Key]
        public int OwnerId { get; set; }
    }
}