/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Felix-Dev/StardewMods
**
*************************************************/

using StardewModdingAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelixDev.StardewMods.FeTK.Framework.Services
{
    /// <summary>
    /// Provides an API to interact with a <see cref="IMailService"/> instance.
    /// </summary>
    internal interface IMailSender : IMailService, IMailObserver
    {
        /// <summary>
        /// Retrieve a mail by its ID and arrival day.
        /// </summary>
        /// <param name="mailId">The ID of the mail.</param>
        /// <param name="arrivalDay">The mail's arrival day in the mailbox of the receiver.</param>
        /// <returns>
        /// A <see cref="Mail"/> instance with the specified <paramref name="mailId"/> and <paramref name="arrivalDay"/> on success;
        /// otherwise, <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The specified <paramref name="mailId"/> is <c>null</c> or does not contain at least one non-whitespace character.
        /// </exception>
        /// <exception cref="ArgumentNullException">The specified <paramref name="arrivalDay"/> is <c>null</c>.</exception>
        Mail GetMailFromId(string mailId, SDate arrivalDay);
    }
}
