/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace SleepInWedding;

/// <summary>
/// Utility functions for this mod.
/// </summary>
internal static class WeddingUtils
{
    /// <summary>
    /// Checks to see if the farmer is supposed to be getting married today.
    /// </summary>
    /// <param name="farmer">farmer.</param>
    /// <returns>True if they're scheduled for a wedding today, false otherwise.</returns>
    internal static bool HasWeddingToday(this Farmer farmer)
        => HasWeddingToday(farmer.UniqueMultiplayerID);

    /// <summary>
    /// Checks to see if the farmer with that specifc multiplayer ID is supposed to be getting married today.
    /// </summary>
    /// <param name="multiplayerID">Multiplayer ID of the farmer.</param>
    /// <returns>True if they're scheduled for a wedding today, false otherwise.</returns>
    internal static bool HasWeddingToday(long multiplayerID)
    {
        if (Game1.weddingsToday.Contains(multiplayerID))
        {
            return true;
        }
        Farmer farmer = Game1.getFarmerMaybeOffline(multiplayerID);
        if (farmer is not null && farmer.team.GetSpouse(multiplayerID) is long spouse)
        {
            return Game1.weddingsToday.Contains(spouse);
        }
        return false;
    }
}
