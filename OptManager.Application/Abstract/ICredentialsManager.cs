using OtpManager.Common.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtpManager.Application.Abstract
{
    public interface ICredentialsManager<TRule> : IApplicationServiceWithRules<TRule> where TRule : class
    {

        /// <summary>
        /// Creates a one-time password for a user.
        /// Password is valid for 30 seconds (configurable in .config)
        /// One valid password at a time is possible for each user.
        /// </summary>
        /// <param name="userId">Unique userId (case insensitive) max(50 char)</param>
        /// <returns>Returns the otp for the user, null if error or invalid input</returns>
        string CreateOtp(string userId);

        /// <summary>
        /// Check if credentials are valid
        /// </summary>
        /// <param name="userId">UserId (case insensitive)</param>
        /// <param name="password">One-time password (case sensitive) valid for 20 seconds (configurable in .config)</param>
        /// <returns></returns>
        bool CheckOtp(string userId, string password);
    }
}
