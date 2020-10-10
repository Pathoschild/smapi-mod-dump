/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

namespace ModUpdateMenu.Menus
{
    /// <summary>Status of the update button.</summary>
    internal enum ButtonStatus
    {
        /// <summary>There are no updates.</summary>
        NoUpdates,

        /// <summary>There are updates.</summary>
        Updates,

        /// <summary>There was an error when checking for updates.</summary>
        Error,

        /// <summary>Updates haven't been checked for yet, or are still being checked.</summary>
        Unknown
    }
}
