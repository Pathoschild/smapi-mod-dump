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
using System.Text;

namespace FelixDev.StardewMods.FeTK.Framework.Services
{
    /// <summary>
    /// Provides data for the <see cref="IMailService.MailOpening"/> event.
    /// </summary>
    public class MailOpeningEventArgs : EventArgs
    {
        /// <summary>
        /// Create a new instance of the <see cref="MailOpeningEventArgs"/> class.
        /// </summary>
        /// <param name="id">The ID of the mail being opened.</param>
        /// <param name="arrivalDay">The mailbox arrival day of the mail being opened.</param>
        /// <param name="content">The content of the mail being opened.</param>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="id"/> is <c>null</c> -or-
        /// the specified <paramref name="arrivalDay"/> is <c>null</c> -or-
        /// the specified <paramref name="content"/> is <c>null</c>.
        /// </exception>
        public MailOpeningEventArgs(string id, SDate arrivalDay, MailContent content)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            ArrivalDay = arrivalDay ?? throw new ArgumentNullException(nameof(arrivalDay));
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        /// <summary>
        /// The ID of the mail being opened.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The mailbox arrival day of the mail being opened.
        /// </summary>
        public SDate ArrivalDay { get; }

        /// <summary>
        /// The content of the mail being opened.
        /// </summary>
        public MailContent Content { get; }
    }
}
