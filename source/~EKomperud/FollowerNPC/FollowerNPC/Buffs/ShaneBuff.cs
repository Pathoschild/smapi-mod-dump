/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/EKomperud/StardewMods
**
*************************************************/

using System;
using StardewValley;

namespace FollowerNPC.Buffs
{
    class ShaneBuff : CompanionBuff
    {
        public ShaneBuff(Farmer farmer, NPC npc, CompanionsManager manager) : base(farmer, npc, manager)
        {
            buff = new Buff(0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 30, "", "");
            buff.description = "Shane may shun organic ingredients in favor of frozen pizza, but he's still good with chickens."+
                               Environment.NewLine+
                               "You gain +3 farming with Shane.";

            statBuffs = new Buff[1];
            statBuffs[0] = new Buff(3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 30, "", "");
            statBuffs[0].description = "+3 Farming" +
                                       Environment.NewLine +
                                       "Source: Shane";
        }
    }
}
