using System;
using StardewValley;

namespace FollowerNPC.Buffs
{
    class ElliottBuff : CompanionBuff
    {
        public ElliottBuff(Farmer farmer, NPC npc) : base(farmer, npc)
        {
            buff = new Buff(0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 30, "", "");
            buff.description =
                "Truly a descendant of Thoreau himself, there's nobody quite"+
                Environment.NewLine+
                "like Elliott to sit down and fish with next to a tranquil pond."+
                Environment.NewLine+
                "You gain +3 to your Fishing skill.";

            statBuffs = new Buff[1];
            statBuffs[0] = new Buff(0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 30, "", "");
            statBuffs[0].description = "+3 Fishing" +
                                       Environment.NewLine +
                                       "Source: Elliot";
        }
    }
}
