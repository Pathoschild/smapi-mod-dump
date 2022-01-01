/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using TehPers.Core.Api.Items;

namespace TehPers.FishingOverhaul.Api.Events
{
    /// <summary>
    /// Event data for a custom fishing event.
    /// </summary>
    public class CustomEventArgs : EventArgs
    {
        /// <summary>
        /// The catch info.
        /// </summary>
        public CatchInfo Catch { get; }

        /// <summary>
        /// The key for the event.
        /// </summary>
        public NamespacedKey EventKey { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomEventArgs"/> class.
        /// </summary>
        /// <param name="catch">The catch info.</param>
        /// <param name="eventKey">The key for the event.</param>
        /// <exception cref="ArgumentNullException">An argument was null.</exception>
        public CustomEventArgs(CatchInfo @catch, NamespacedKey eventKey)
        {
            this.Catch = @catch ?? throw new ArgumentNullException(nameof(@catch));
            this.EventKey = eventKey;
        }
    }
}