/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

namespace TehPers.FishingOverhaul.Api
{
    /// <summary>
    /// The state of the treasure in the fishing minigame.
    /// </summary>
    public enum TreasureState
    {
        /// <summary>
        /// No treasure can be caught.
        /// </summary>
        None,
        /// <summary>
        /// Treasure can be caught, but it has not yet been caught.
        /// </summary>
        NotCaught,
        /// <summary>
        /// Treasure was caught.
        /// </summary>
        Caught,
    }
}