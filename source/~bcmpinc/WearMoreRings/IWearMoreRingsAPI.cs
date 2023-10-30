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

    public interface IWearMoreRingsAPI_2 {
        /// <summary>
        /// Get the mod's config setting for how many rings can be equipped.
        /// 
        /// Note that this value is not synchronized in multiplayer, so its only valid for the current player (Game1.player).
        /// </summary>
        /// <returns>Config setting for how many rings the local player can wear.</returns>
        int RingSlotCount();


        /// <summary>
        /// Get the ring that the local player has equipped in the given slot. 
        /// </summary>
        /// <param name="slot">The ring equipment slot being queried. Ranging from 0 to RingSlotCount()-1.</param>
        /// <returns>The ring equipped in the given slot or null if its empty.</returns>
        StardewValley.Objects.Ring GetRing(int slot);

        /// <summary>
        /// Equip a new ring in the given slot. Note that this overwrites the previous ring in that slot. Use null to remove the ring.
        /// </summary>
        /// <param name="slot">The ring equipment slot being queried. Ranging from 0 to RingSlotCount()-1.</param>
        /// <param name="ring">The new ring being equipped. Can be null to unequip the current ring.</param>
        void SetRing(int slot, StardewValley.Objects.Ring ring);

    }
}
