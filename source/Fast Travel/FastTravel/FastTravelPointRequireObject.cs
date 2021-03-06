/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DeathGameDev/SDV-FastTravel
**
*************************************************/

using System;

namespace FastTravel
{
    /// <summary>A fast travel point on the map.</summary>
    [Serializable]
    public struct FastTravelPointRequireObject
    {
        /// <summary>List of required mail flags, in <see cref="StardewValley.Farmer.mailReceived"/>.</summary>
        public string[] mails;
    }
}