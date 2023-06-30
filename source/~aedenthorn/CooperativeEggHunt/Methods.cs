/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;

namespace CooperativeEggHunt
{
    public partial class ModEntry
    {
        private static int GetEggs(NPC npc)
        {
            if(npc == null) 
                return -1;
            int eggs = Game1.random.Next(Config.NPCMinEggs, Config.NPCMaxEggs + 1);
            if(Config.PointsPerEgg > 0)
            {
                foreach (Farmer temp in Game1.getOnlineFarmers())
                {
                    if (temp.friendshipData.TryGetValue(npc.Name, out var f))
                    {
                        eggs += f.Points / Config.PointsPerEgg;
                    }
                }
            }
            if(Config.EggsPerTalk > 0 && npc.modData.TryGetValue(talkedKey, out string talked) && int.TryParse(talked, out int e))
            {
                eggs += e * Config.EggsPerTalk;
            }
            SMonitor.Log($"NPC {npc.Name} got {eggs} eggs");
            return eggs;
        }
    }
}