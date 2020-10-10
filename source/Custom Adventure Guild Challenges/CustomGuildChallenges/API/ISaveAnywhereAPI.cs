/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/defenthenation/StardewMod-CustomSlayerChallenges
**
*************************************************/

using System;

namespace CustomGuildChallenges.API
{
    public interface ISaveAnywhereAPI
    {
        /*********
        ** Events
        *********/
        /// <summary>
        ///     Event that fires before game save
        /// </summary>
        event EventHandler BeforeSave;
        /// <summary>
        ///     Event that fires after game save
        /// </summary>
        event EventHandler AfterSave;
        /// <summary>
        ///     Event that fires after game load
        /// </summary>
        event EventHandler AfterLoad;
    }
}
