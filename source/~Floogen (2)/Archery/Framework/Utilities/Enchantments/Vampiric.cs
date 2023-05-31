/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using Archery.Framework.Interfaces.Internal;
using StardewValley;
using System;
using System.Collections.Generic;

namespace Archery.Framework.Utilities.Enchantments
{
    public class Vampiric
    {
        private static float _defaultPercentage = 0.1f;

        internal static string GetDescription(List<object> arguments)
        {
            return $"Restores {GetPercentage(arguments)}% of the damage done as health.";
        }

        internal static bool HandleEnchantment(IEnchantment enchantment)
        {
            if (enchantment.Farmer is null || enchantment.Monster is null || enchantment.DamageDone is null)
            {
                return false;
            }
            var farmer = enchantment.Farmer;

            // Take % of the damage done and restore the farmer's health
            int amountToHeal = (int)(enchantment.DamageDone.Value * (GetPercentage(enchantment.Arguments) / 100f));
            farmer.health = amountToHeal + farmer.health > farmer.maxHealth ? farmer.maxHealth : amountToHeal + farmer.health;

            return false;
        }

        private static int GetPercentage(List<object> arguments)
        {
            var percentage = _defaultPercentage;
            if (arguments is not null && arguments.Count > 0)
            {
                try
                {
                    percentage = Convert.ToSingle(arguments[0]);
                }
                catch (Exception ex)
                {
                    Archery.monitor.LogOnce($"Failed to process percentage argument for Archery/PeacefulEnd.Archery/Vampiric! See the log for details.", StardewModdingAPI.LogLevel.Error);
                    Archery.monitor.LogOnce($"Failed to process percentage argument for Archery/PeacefulEnd.Archery/Vampiric:\n{ex}", StardewModdingAPI.LogLevel.Trace);
                }
            }
            return (int)(Utility.Clamp(percentage, 0, 1) * 100);
        }
    }
}
