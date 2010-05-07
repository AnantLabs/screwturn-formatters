using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Keeper.Garrett.ScrewTurn.Utility
{
    public class Avatar
    {
        /// <summary>
        /// Displays the gravatar of the user.
        /// </summary>
        public static string GenerateAvatarLink(string _email)
        {
            string retval = string.Empty;

            _email = (string.IsNullOrEmpty(_email) == true ? "Missing User Email" : _email);
            retval = string.Format(@"<img src=""http://www.gravatar.com/avatar/{0}?d=identicon"" alt=""Gravatar"" height=""80"" width=""80"" />", GetAvatarHash(_email));

            return retval;
        }

        /// <summary>
        /// Gets the gravatar hash of an email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns>The hash.</returns>
        private static string GetAvatarHash(string email)
        {
            MD5 md5 = MD5.Create();
            byte[] bytes = md5.ComputeHash(Encoding.ASCII.GetBytes(email.ToLowerInvariant()));

            StringBuilder sb = new StringBuilder(100);
            foreach (byte b in bytes)
            {
                sb.AppendFormat("{0:x2}", b);
            }

            return sb.ToString();
        }
    }
}
