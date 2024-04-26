/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using StardewValley;
using System;

namespace StardewRPG
{
    public partial class ModEntry
    {
        private static bool BuffsDisplay_addOtherBuff_Prefix(ref Buff buff)
        {
            if (!Config.EnableMod)
                return true;
            foreach(int i in buff.buffAttributes)
            {
                if (i > 0)
                    return true;
            }
            if(Config.ConRollToResistDebuff && Game1.random.Next(20) < GetStatValue(Game1.player, "con", Config.BaseStatValue))
            {
                SMonitor.Log($"Resisted debuff {buff.which}");
                return false;
            }
            var newDur = (int)Math.Round(buff.millisecondsDuration * (1 - GetStatMod(GetStatValue(Game1.player, "con", Config.BaseStatValue)) * Config.ConDebuffDurationBonus));
            SMonitor.Log($"Modifying buff duration {buff.millisecondsDuration} => {newDur}");
            buff.millisecondsDuration = newDur;
            return true;
        }
    }
}