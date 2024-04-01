/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace CombatOverhaulMod.FightStamina
{
    public static class Extensions
    {
        public static float GetFightStamina(this Farmer farmer)
        {
            return farmer.get_FightStaminaValue().Value;
        }

        public static float GetMaxFightStamina(this Farmer farmer)
        {
            return farmer.get_FightStaminaMaxValue().Value;
        }

        public static void AddFightStamina(this Farmer farmer, float amt)
        {
            var val = farmer.get_FightStaminaValue();
            val.Value = Math.Min(Math.Max(0, val.Value + amt), farmer.GetMaxFightStamina()); ;
        }

        public static void SetFightStamina(this Farmer farmer, float val)
        {
            farmer.get_FightStaminaValue().Value = Math.Min(Math.Max(0, val), farmer.GetMaxFightStamina());
        }

        public static void SetMaxFightStamina(this Farmer farmer, float val)
        {
            farmer.get_FightStaminaMaxValue().Value = Math.Max(1, val);
        }
    }
}
