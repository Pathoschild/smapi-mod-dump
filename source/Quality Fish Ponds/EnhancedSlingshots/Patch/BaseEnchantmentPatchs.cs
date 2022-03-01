/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/YTSC/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using EnhancedSlingshots.Enchantments;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhancedSlingshots.Patch
{
    [HarmonyPatch(typeof(BaseEnchantment))]
    public static class BaseEnchantmentPatchs
    {
        private static IMonitor Monitor;

        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(BaseEnchantment.GetEnchantmentFromItem))]
        public static void GetEnchantmentFromItem_Postfix(ref BaseEnchantment __result, Item base_item, Item item)
        {        
            if (base_item != null && base_item is Slingshot sling && sling.CurrentParentTileIndex == 34 && Utility.IsNormalObjectAtParentSheetIndex(item, 896))
                __result = new GalaxySoulEnchantment();            
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(BaseEnchantment.GetAvailableEnchantments))]
        public static void GetAvailableEnchantments_Postfix(ref List<BaseEnchantment> __result)
        {
            //__result.Add(new PotentEnchantment());
            __result.Add(new AutomatedEnchantment());          
            __result.Add(new ExpertEnchantment());
            __result.Add(new HunterEnchantment());            
            __result.Add(new MinerEnchantment());          
            __result.Add(new PreciseEnchantment());
            __result.Add(new SwiftEnchantment());
            __result.Add(new Enchantments.PreservingEnchantment());
            __result.Add(new Enchantments.BugKillerEnchantment());
            __result.Add(new Enchantments.VampiricEnchantment());
        }
    }
}
