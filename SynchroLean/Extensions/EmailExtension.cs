using System.Net.Mail;

namespace SynchroLean.Extensions
{
    public static class EmailExtension
    {
        public static bool TryNormalizeEmail(this string readIn, out string writeTo)
        {
            try 
            {
                var address = new MailAddress(readIn);
                writeTo = address.User.ToLower() + "@" + address.Host.ToLower();
                return true;
            }
            catch 
            {
                writeTo = "";
                return false;
            }
        }
    }
}
