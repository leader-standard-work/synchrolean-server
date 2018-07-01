using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SynchroLean
{
    interface IUserLogin
    {
        /// <summary>
        /// Is the login valid, right now?
        /// </summary>
        bool testValid();
        
        /// <summary>
        /// Get an ID for the user for use as a parameter.
        /// </summary>
        string UserID { get; }
    }
}
