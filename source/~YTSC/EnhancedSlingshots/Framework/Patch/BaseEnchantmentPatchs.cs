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
using EnhancedSlingshots.Framework.Enchantments;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using System.Collections.Generic;

namespace EnhancedSlingshots.Framework.Patch
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
            if (base_item != null && base_item is Slingshot sling && sling.InitialParentTileIndex == Slingshot.galaxySlingshot && Utility.IsNormalObjectAtParentSheetIndex(item, 896))
                __result = new GalaxySoulEnchantment();            
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(BaseEnchantment.GetAvailableEnchantments))]
        public static void GetAvailableEnchantments_Prefix(List<BaseEnchantment> ____enchantments, out bool __state)
        {
            __state = ____enchantments is null;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(BaseEnchantment.GetAvailableEnchantments))]
        public static void GetAvailableEnchantments_Postfix(ref List<BaseEnchantment> __result, bool __state)
        {
            if (__state)
            {
                __result.AddRange(new BaseEnchantment[]
                {
                     //new GeminiEnchantment(),
                    new MagneticEnchantment(),
                    new AutomatedEnchantment(),
                    new ExpertEnchantment(),
                    new HunterEnchantment(),
                    new MinerEnchantment(),
                    new PreciseEnchantment(),
                    new SwiftEnchantment(),
                    new Enchantments.PreservingEnchantment(),
                    new Enchantments.BugKillerEnchantment(),
                    new Enchantments.VampiricEnchantment()
                });               
            }           
        }
    }
}
