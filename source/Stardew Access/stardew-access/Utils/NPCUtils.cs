/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using StardewValley;

namespace stardew_access.Utils
{
    internal static class NPCUtils
    {
        internal static void GhostNPC(NPC? npc, bool sameTile = false, int delay = 100)
        {
            if (npc != null && !npc.IsInvisible)
            {
                if (sameTile && (npc.getTileLocation() != Game1.player.getTileLocation())) return;
                npc.IsInvisible = true;
                _ = UnghostNPC(npc, delay);
            }
        }

        internal static async Task UnghostNPC(NPC? npc, int delay)
        {
            await Task.Delay(delay);
            if (npc is not null)
            {
                npc.IsInvisible = false;
            }
        }
    }
}
