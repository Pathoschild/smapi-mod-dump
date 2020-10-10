/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/CombatLevelDamageScaler
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatLevelDamageScaler.Overrides
{
    internal class DamageMonsterHook
    {
        internal static void Prefix(ref int minDamage, ref int maxDamage, Farmer who)
        {
            float scale = 1.0f + who.CombatLevel * Mod.Config.DamageScalePerLevel;
            minDamage = (int)(minDamage * scale);
            maxDamage = (int)(maxDamage * scale);
        }
    }
}
