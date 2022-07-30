/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/YTSC/StardewValleyMods
**
*************************************************/

using EnhancedSlingshots.Framework.Enchantments;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhancedSlingshots.Framework
{
    public enum EnchantmentKey
    {
        Invalid = 0,
        Automated,
        Expert,
        Gemini,
        Hunter,
        Magnetic,
        Miner,
        Precise,       
        Swift,
        Preserving,
        BugKiller,
        Vampiric,
    }

    public static class SlingshotEnchantment
    {
        public static Dictionary<EnchantmentKey, BaseEnchantment> Enchantments = new()
        {
            { EnchantmentKey.Automated, new AutomatedEnchantment() },
            { EnchantmentKey.Expert, new ExpertEnchantment() },
            { EnchantmentKey.Gemini, new GeminiEnchantment() },
            { EnchantmentKey.Hunter, new HunterEnchantment() },
            { EnchantmentKey.Magnetic, new MagneticEnchantment() },
            { EnchantmentKey.Miner, new MinerEnchantment() },
            { EnchantmentKey.Precise, new PreciseEnchantment() },
            { EnchantmentKey.Swift, new SwiftEnchantment() },
            { EnchantmentKey.Preserving, new Enchantments.PreservingEnchantment() },
            { EnchantmentKey.BugKiller, new Enchantments.BugKillerEnchantment() },
            { EnchantmentKey.Vampiric, new Enchantments.VampiricEnchantment() },
        };

        public static EnchantmentKey GetKeyByEnchantmentType(BaseEnchantment enchantment)
        {
            return Enchantments.FirstOrDefault(x => x.Value.GetType() == enchantment.GetType()).Key;
        }
    }
}
