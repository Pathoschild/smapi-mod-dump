/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

namespace ModUpdateMenu.Updates
{
    /// <summary>An update status</summary>
    public enum UpdateStatus
    {
        /// <summary>Update checking was skipped.</summary>
        Skipped,

        /// <summary>The mod version is equal to remote version.</summary>
        UpToDate,

        /// <summary>The mod version is less than the remote version.</summary>
        OutOfDate
    }
}
