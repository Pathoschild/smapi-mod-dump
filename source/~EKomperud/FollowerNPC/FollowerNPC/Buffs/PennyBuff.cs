using System;
using StardewValley;

namespace FollowerNPC.Buffs
{
    class PennyBuff : CompanionBuff
    {
        public PennyBuff(Farmer farmer, NPC npc) : base(farmer, npc)
        {
            buff = new Buff(0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 30, "", "");
            buff.description = "Penny's a true homesteader, and she's more than willing to help out on the farm!" +
                               Environment.NewLine+
                               "You gain +3 Farming with her at your side.";

            statBuffs = new Buff[1];
            statBuffs[0] = new Buff(3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 30, "", "");
            statBuffs[0].description = "+3 Farming" +
                                       Environment.NewLine +
                                       "Source: Penny";
        }
    }
}
