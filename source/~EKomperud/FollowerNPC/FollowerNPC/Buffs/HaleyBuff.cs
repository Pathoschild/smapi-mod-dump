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
                               "Your luck is increased +2! She's also an expert at catching fond" +
                                Environment.NewLine +
                               "memories in her lens, increasing your exp gained by 5% (minimum +1).";
            statBuffs = new Buff[1];
            statBuffs[0] = new Buff(0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 30, "", "");
            statBuffs[0].description = "+2 Luck" + Environment.NewLine + "Source: Haley";
            Patches.increaseExperience = true;
            Patches.farmer = buffOwner.Name;
        }

        public override void RemoveAndDisposeCompanionBuff()
        {
            base.RemoveAndDisposeCompanionBuff();
            Patches.increaseExperience = false;
            Patches.farmer = null;
        }
    }
}
