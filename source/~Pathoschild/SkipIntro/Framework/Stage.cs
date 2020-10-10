/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

namespace Pathoschild.Stardew.SkipIntro.Framework
{
    /// <summary>A step in the mod logic.</summary>
    internal enum Stage
    {
        /// <summary>No action needed.</summary>
        None,

        /// <summary>Skip the initial intro.</summary>
        SkipIntro,

        /// <summary>The co-op menu is waiting for a connection.</summary>
        WaitingForConnection
    }
}
