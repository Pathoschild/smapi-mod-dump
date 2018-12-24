using System;
using StardewValley;

namespace FollowerNPC.Buffs
{
    class LeahBuff : CompanionBuff
    {
        public LeahBuff(Farmer farmer) : base(farmer)
        {
            buff = new Buff(0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 30, "", "");
            buff.description = "Leah always seems to smell like chopped wood and fall mushrooms."+
                               Environment.NewLine+
                               "You gain +3 Foraging while hanging out with her!";

            statBuffs = new Buff[1];
            statBuffs[0] = new Buff(0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 30, "", "");
            statBuffs[0].description = "+3 Foraging" +
                                       Environment.NewLine +
                                       "Source: Leah";
        }
    }
}
