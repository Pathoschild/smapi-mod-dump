/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewRoguelike.Enchantments;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(BaseEnchantment), "GetEnchantmentFromItem")]
    internal class FixEnchantmentRandomPatch
    {
        public static bool Prefix(ref BaseEnchantment __result, Item base_item, Item item)
        {
            if (base_item == null || (base_item is MeleeWeapon && !((MeleeWeapon)base_item).isScythe()))
            {
                if (base_item != null && base_item is MeleeWeapon && ((MeleeWeapon)base_item).isGalaxyWeapon() && Utility.IsNormalObjectAtParentSheetIndex(item, 896))
                    __result = new GalaxySoulEnchantment();
                else if (Utility.IsNormalObjectAtParentSheetIndex(item, 60))
                    __result = new EmeraldEnchantment();
                else if (Utility.IsNormalObjectAtParentSheetIndex(item, 62))
                    __result = new AquamarineEnchantment();
                else if (Utility.IsNormalObjectAtParentSheetIndex(item, 64))
                    __result = new RubyEnchantment();
                else if (Utility.IsNormalObjectAtParentSheetIndex(item, 66))
                    __result = new AmethystEnchantment();
                else if (Utility.IsNormalObjectAtParentSheetIndex(item, 68))
                    __result = new TopazEnchantment();
                else if (Utility.IsNormalObjectAtParentSheetIndex(item, 70))
                    __result = new JadeEnchantment();
                else if (Utility.IsNormalObjectAtParentSheetIndex(item, 72))
                    __result = new DiamondEnchantment();
            }

            if (Utility.IsNormalObjectAtParentSheetIndex(item, 74))
                __result = Utility.GetRandom(BaseEnchantment.GetAvailableEnchantmentsForItem(base_item as Tool), Game1.random);

            return false;
        }
    }

    [HarmonyPatch(typeof(BaseEnchantment), "GetAvailableEnchantments")]
    internal class GetAvailableEnchantmentsPatch
    {
        private readonly static List<BaseEnchantment> _enchantments = new();

        public static bool Prefix(ref List<BaseEnchantment> __result)
        {
            if (_enchantments.Count == 0)
            {
                _enchantments.Add(new VampiricEnchantment());
                _enchantments.Add(new ArtfulEnchantment());
                _enchantments.Add(new CrusaderEnchantment());
                _enchantments.Add(new CustomBugKillerEnchantment());
                _enchantments.Add(new SlimeKillerEnchantment());
                _enchantments.Add(new StarShooterEnchantment());
            }

            __result = _enchantments;
            return false;
        }
    }
}
