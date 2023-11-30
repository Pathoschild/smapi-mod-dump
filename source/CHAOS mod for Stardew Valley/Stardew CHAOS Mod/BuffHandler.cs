/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DevWithMaj/Stardew-CHAOS
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stardew_CHAOS_Mod
{
    internal class BuffHandler
    {

        public enum Buffs : int
        {
            DARKNESS=26,
            FROZEN=19,
            SLIME=13,
            TIPSY=17,
            BURNED=12,
            SPEED=9
        }

        public static void GiveBuff(Buffs buffId, int duration)
        {
            Game1.playSound("debuffHit");
            int id = (int)buffId;
            Buff b = new Buff(id);
            b.millisecondsDuration = duration;
            Game1.buffsDisplay.addOtherBuff(b);
        }

        public static void GiveBuff(Buffs buffId, int duration, int strength)
        {
            Game1.playSound("debuffHit");
            int id = (int)buffId;
            Buff b = new Buff(id);
            b.millisecondsDuration = duration;
            if (buffId == Buffs.SPEED)
            {
                b.buffAttributes[9] = strength;
            }
            Game1.buffsDisplay.addOtherBuff(b);
        }

        public static void GiveBuff(int buffId, int duration)
        {
            Game1.playSound("debuffHit");
            Buff b = new Buff(buffId);
            b.millisecondsDuration = duration;
            Game1.buffsDisplay.addOtherBuff(b);
        }

    }
}
