/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bcmpinc/StardewHack
**
*************************************************/

using System;

namespace StardewHack.WearMoreRings
{
    
    [Obsolete("Wear More Rings now stores rings in a combined ring in the player's left ring slot. Using this API should therefore no longer be necessary for interoperability between mods that do stuff with rings and the Wear More Rings mod.")]
    public interface IWearMoreRingsAPI {
        /// <summary>
        /// Count how many of the specified ring type the given player has equipped. This includes the vanilla left & right rings.
        /// Furthermore, it includes the rings contained in combined rings, but excludes the combined rings itself.
        /// </summary>
        /// <returns>How many of the specified ring the given player has equipped.</returns>
        /// <param name="f">The farmer/farmhand whose inventory is being checked. For the local player, use Game1.player.</param>
        /// <param name="which">The parentSheetIndex of the ring.</param>
        int CountEquippedRings(StardewValley.Farmer f, int which);
        
        /// <summary>
        /// Returns a list of all rings the player has equipped. This includes the vanilla left & right rings.
        /// Furthermore, it includes the rings contained in combined rings, but excludes the combined rings itself.
        /// </summary>
        /// <returns>A list of all equiped rings.</returns>
        /// <param name="f">The farmer/farmhand whose inventory is being checked. For the local player, use Game1.player.</param>
        System.Collections.Generic.IEnumerable<StardewValley.Objects.Ring> GetAllRings(StardewValley.Farmer f);
    }
}
