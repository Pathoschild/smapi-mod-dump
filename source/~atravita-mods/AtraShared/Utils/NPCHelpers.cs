/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace AtraShared.Utils;

/// <summary>
/// Helper methods for NPCs.
/// </summary>
public static class NPCHelpers
{
    /// <summary>
    /// Gets all NPCs (that are villagers).
    /// </summary>
    /// <returns>IEnumerable of villagers.</returns>
    /// <remarks>Unlike <see cref="Utility.getAllCharacters"/>, skips nulls, avoids non-villagers, and doesn't search the farm (where npcs aren't supposed to be anyways).</remarks>
    public static IEnumerable<NPC> GetNPCs()
        => Game1.locations.SelectMany(loc => loc.characters.Where(npc => npc is not null && npc.isVillager()));
}
