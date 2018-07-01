using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SynchroLean.Models
{
    public class UserToken: IUserLogin
    {
        //These really should all be read only, we might want a code-facing interface
        //that exposes better types (enumerations for the methods, a URI for the issuer-id,
        //and a boolean check if a login is valid.

        /// <summary>
        /// A URL pointing to the issuer of the identity token.
        /// </summary>
        [Required]
        [DataType(DataType.Url)]
        string IssuerID { get; set; }

        /// <summary>
        /// A unique identifier for the subject.
        /// </summary>
        [Required]
        [StringLength(255)]
        string SubjectID { get; set; }

        /// <summary>
        /// The intended recipients of this token. The server should 
        /// check that it is one of them!
        /// </summary>
        [Required]
        HashSet<string> Audiences { get; }

        /// <summary>
        /// The last time when the token will still be valid. Do not use the token after.
        /// </summary>
        [Required]
        DateTime ExpirationTime { get; }

        /// <summary>
        /// The time when the token was issued.
        /// </summary>
        [Required]
        DateTime IssueTime { get; }

        /// <summary>
        /// The time when the user successfully logged in to the service issuing the token.
        /// </summary>
        [Required]
        DateTime AuthenticationTime { get; }

        /// <summary>
        /// A unique identifier for a session. A client claiming to have this authorization token must
        /// also demonstrate knowledge of the nonce. This is optional.
        /// </summary>
        string Nonce { get; }

        /// <summary>
        /// How confident we are that the user is who they are (roughly). If 0, could have been an automatic
        /// login via a cookie or some other saved state. The ID provider is not required to send
        /// this value, it should default to 0 in the model that case.
        /// </summary>
        uint AuthenticationClass { get; }

        //I don't think we actually need this, we could potentially ban some methods known to be insecure?
        /// <summary>
        /// A reference to the exact methods that were used to authenticate the user (i.e., was it a password, was it
        /// a fingerprint, was it a one-time-pad, was it a PIN?). This is optional.
        /// </summary>
        HashSet<string> AuthenticationMethodReference;

        /// <summary>
        /// The party to which this token was issued. It should only be used if there is a single audience
        /// value, and it is only useful if the authorized party is different from the audience.
        /// </summary>
        string AuthorizedParty { get; }

        string IUserLogin.UserID { get { return this.SubjectID; } }

        bool IUserLogin.testValid() { 
            var validRightNow = DateTime.Now <= ExpirationTime;
            //Put in stuff here for validating that the token applies to us
            // and is from one of our trusted providers...
            return validRightNow;
        }

        IEnumerable<DayOfWeek> convertDaysOfWeekFromBits(sbyte bits)
        {
            for (int i = 0; i < 7; i++)
            {
                if ((bits & 1) > 0) yield return (DayOfWeek)i;
                bits >>= 1;
            }
        }

        sbyte convertDaysOfWeekToBits(IEnumerable<DayOfWeek> days)
        {
            sbyte result = 0;
            foreach(DayOfWeek weekday in days)
            {
                result |= (sbyte)(1 << (sbyte)(weekday));
            }
            return result;
        }
    }
}
