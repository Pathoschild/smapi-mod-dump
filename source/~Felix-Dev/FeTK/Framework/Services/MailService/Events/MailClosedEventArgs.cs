/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Felix-Dev/StardewMods
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Text;

namespace FelixDev.StardewMods.FeTK.Framework.Services
{
    /// <summary>
    /// Provides data for the <see cref="IMailService.MailClosed"/> event.
    /// </summary>
    public class MailClosedEventArgs : EventArgs
    {
        /// <summary>
        /// Create a new instance of the <see cref="MailClosedEventArgs"/> class.
        /// </summary>
        /// <param name="id">The ID of the mail which was closed.</param>
        /// <param name="interactionRecord">Information about how the player interacted with the mail content.</param>
        /// <exception cref="ArgumentNullException">
        /// The given <paramref name="id"/> is <c>null</c> -or-
        /// the given <paramref name="interactionRecord"/> is <c>null</c>.
        /// </exception>
        public MailClosedEventArgs(string id, MailInteractionRecord interactionRecord)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            InteractionRecord = interactionRecord ?? throw new ArgumentNullException(nameof(interactionRecord));
        }

        /// <summary>
        /// The ID of the closed mail.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Get information about how the player interacted with the mail.
        /// </summary>
        public MailInteractionRecord InteractionRecord { get; }
    }
}
