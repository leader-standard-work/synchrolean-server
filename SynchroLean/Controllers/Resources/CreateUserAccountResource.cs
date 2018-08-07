using System;

namespace SynchroLean.Controllers.Resources
{
    /// <summary>
    /// This class is used to retrieve/send user identification information to/from mobile or web app
    /// </summary>
    public class CreateUserAccountResource : UserAccountResource
    {
        /// <value>Gets and sets user password</value>
        public string Password { get; set; }
    }
}
