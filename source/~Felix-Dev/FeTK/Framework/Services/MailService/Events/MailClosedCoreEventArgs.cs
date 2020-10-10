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
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelixDev.StardewMods.FeTK.Framework.Services
{
    /// <summary>
    /// Provides data for the <see cref="MailManager.MailClosed"/> event.
    /// </summary>
    internal class MailClosedCoreEventArgs : MailClosedEventArgs
    {
        /// <summary>
        /// Create a new instance of the <see cref="MailClosedCoreEventArgs"/> class.
        /// </summary>
        /// <param name="id">The ID of the mail to be closed.</param>
        /// <param name="arrivalDay">The mail's day of arrival in the receiver's mailbox.</param>
        /// <param name="interactionRecord">Information about how the player interacted with the mail content.</param>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="id"/> is <c>null</c> -or-
        /// the specified <paramref name="arrivalDay"/> is <c>null</c> -or-
        /// the specified <paramref name="interactionRecord"/> is <c>null</c>.
        /// </exception>
        public MailClosedCoreEventArgs(string id, SDate arrivalDay, MailInteractionRecord interactionRecord) 
            : base(id, interactionRecord)
        {
            ArrivalDay = arrivalDay ?? throw new ArgumentNullException(nameof(arrivalDay));
        }

        /// <summary>
        /// The mail's day of arrival in the receiver's mailbox.
        /// </summary>
        public SDate ArrivalDay { get; }
    }
}
