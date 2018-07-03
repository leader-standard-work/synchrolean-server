using System;

namespace SynchroLean.Controllers.Resources
{
    public class UserAccountResource
    {
        public int OwnerId { get; set; }
        public string FirstName { get; set; }
        public string LastName  { get; set; }
        public string Email { get; set; }
    }
}