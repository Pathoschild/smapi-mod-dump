/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using StardewModdingAPI;
using System.Collections.Generic;

namespace ModUpdateMenu.Updates
{
    /// <summary>Can be notified about SMAPI/mod statuses.</summary>
    internal interface INotifiable
    {
        /*********
        ** Methods
        *********/
        /// <summary>Notify about mod statuses.</summary>
        /// <param name="statuses">The mod status.</param>
        void Notify(IList<ModStatus> statuses);

        /// <summary>Notifies about the SMAPI update version.</summary>
        /// <param name="version">The SMAPI update version.</param>
        void NotifySMAPI(ISemanticVersion version);
    }
}
