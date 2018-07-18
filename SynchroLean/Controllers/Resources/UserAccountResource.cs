using System;

namespace SynchroLean.Controllers.Resources
{
    /// <summary>
    /// This class is used to retrieve/send user identification information to/from mobile or web app
    /// </summary>
    public class UserAccountResource
    {
        /// <value>Gets and sets user id number</value>
        public int OwnerId { get; set; }
        // /// <value>Gets and sets team id number of user</value>
        // public int TeamId { get; set; }
        /// <value>Gets and sets user first name</value>
        public string FirstName { get; set; }
        /// <value>Gets and sets user last name</value>
        public string LastName  { get; set; }
        /// <value>Gets and sets user email</value>
        public string Email { get; set; }
        /// <value>Gets and sets account active/inactive state</value>
        public bool IsDeleted { get; set; }
    }
}