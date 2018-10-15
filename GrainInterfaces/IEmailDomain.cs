using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Orleans;

namespace GrainInterfaces
{
    /// <summary>
    /// Contains breached emails for a specific domain.
    /// </summary>
    public interface IEmailDomain : IGrainWithStringKey
    {
        /// <summary>
        /// Checks if an email has been breached.
        /// </summary>
        /// <param name="email">The email to be checked.</param>
        /// <returns></returns>
        Task<bool> CheckEmailBreached(string email);

        /// <summary>
        /// Returns a list of all the breached emails in this domain.
        /// </summary>
        /// <returns></returns>
        Task<List<string>> GetBreachedEmails();

        /// <summary>
        /// Adds an email to the breached email list.
        /// </summary>
        /// <param name="email">The email to be added.</param>
        /// <returns></returns>
        Task AddBreachedEmail(string email);
    }
}
