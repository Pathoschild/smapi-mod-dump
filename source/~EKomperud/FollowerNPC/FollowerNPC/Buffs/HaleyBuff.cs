using System;
using StardewValley;

namespace FollowerNPC.Buffs
{
    class HaleyBuff: CompanionBuff
    {
        public HaleyBuff(Farmer farmer, NPC npc, CompanionsManager manager) : base(farmer, npc, manager)
        {
            buff = new Buff(0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 30, "", "");
            buff.description = "When Haley's around, good things just seem to happen more often." + 
                               Environment.NewLine + 
                               "Your luck is increased +3!";
            statBuffs = new Buff[1];
            statBuffs[0] = new Buff(0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 30, "", "");
            statBuffs[0].description = "+3 Luck" + Environment.NewLine + "Source: Haley";
        }

    }
}
