/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Felix-Dev/StardewMods
**
*************************************************/

using FelixDev.StardewMods.FeTK.Framework.Services;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Text;

namespace FelixDev.StardewMods.FeTK.Framework.UI
{
    /// <summary>
    /// Provides data for the <see cref="LetterViewerMenuWrapper.MenuClosed"/> event.
    /// </summary>
    public class LetterViewerMenuClosedEventArgs : EventArgs
    {
        /// <summary>
        /// Create a new instance of the <see cref="LetterViewerMenuClosedEventArgs"/> class.
        /// </summary>
        /// <param name="mailId">The ID of the mail to be closed.</param>
        /// <param name="selectedItems">Sets the items of the mail which were selected. Can be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="mailId"/> is <c>null</c> -or-
        /// the specified <paramref name="interactionRecord"/> is <c>null</c>.
        /// </exception>
        public LetterViewerMenuClosedEventArgs(string mailId, MailInteractionRecord interactionRecord)
        {
            MailId = mailId ?? throw new ArgumentNullException(nameof(mailId));
            InteractionRecord = interactionRecord ?? throw new ArgumentNullException(nameof(interactionRecord));
        }

        /// <summary>
        /// The ID of the closed mail.
        /// </summary>
        public string MailId { get; }

        /// <summary>
        /// Get information about how the player interacted with the mail.
        /// </summary>
        public MailInteractionRecord InteractionRecord { get; }
    }
}
