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
using System;
using System.Collections.Generic;

namespace Archery.Framework.Utilities.Enchantments
{
    public class Shock
    {
        private static int _defaultStunTime = 1000;

        internal static string GetDescription(List<object> arguments)
        {
            var stunTime = GetStunTime(arguments) / 1000;
            if (stunTime > 0)
            {
                return $"Briefly stuns enemies upon impact for {stunTime} second(s).";
            }

            return $"Briefly stuns enemies upon impact.";
        }

        internal static bool HandleEnchantment(IEnchantment enchantment)
        {
            if (enchantment.Farmer is null || enchantment.Monster is null)
            {
                return false;
            }
            var monster = enchantment.Monster;

            // Stun the monster for 1 second
            monster.stunTime += GetStunTime(enchantment.Arguments);

            return false;
        }

        private static int GetStunTime(List<object> arguments)
        {
            var stunTime = _defaultStunTime;
            if (arguments is not null && arguments.Count > 0)
            {
                try
                {
                    stunTime = Convert.ToInt32(arguments[0]);
                }
                catch (Exception ex)
                {
                    Archery.monitor.LogOnce($"Failed to process percentage argument for Archery/PeacefulEnd.Archery/Shock! See the log for details.", StardewModdingAPI.LogLevel.Error);
                    Archery.monitor.LogOnce($"Failed to process percentage argument for Archery/PeacefulEnd.Archery/Shock:\n{ex}", StardewModdingAPI.LogLevel.Trace);
                }
            }
            return stunTime;
        }
    }
}
