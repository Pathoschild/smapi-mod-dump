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
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EnhancedSlingshots.Patch
{
	[HarmonyPatch(typeof(Tool))]
    public static class ToolPatchs
    {
        private static IMonitor Monitor;

        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

		[HarmonyPostfix]
		[HarmonyPatch(nameof(Tool.Forge))]
		public static void Forge_Postfix(Tool __instance, ref bool __result, Item item, bool count_towards_stats = false)
        {			
			BaseEnchantment enchantment = BaseEnchantment.GetEnchantmentFromItem(__instance, item);
			if (__instance is Slingshot && enchantment != null)
			{
				if (enchantment is GalaxySoulEnchantment && __instance is Slingshot sling && sling.CurrentParentTileIndex == 34 && sling.GetEnchantmentLevel<GalaxySoulEnchantment>() >= 3)
				{
					__instance.CurrentParentTileIndex = ModEntry.Instance.config.InfinitySlingshotId;
					__instance.InitialParentTileIndex = ModEntry.Instance.config.InfinitySlingshotId;
					__instance.IndexOfMenuItemView = ModEntry.Instance.config.InfinitySlingshotId;
					string[] slingData = Game1.content.Load<Dictionary<int, string>>("Data\\weapons")[__instance.InitialParentTileIndex].Split('/');
					__instance.BaseName = slingData[0];
					__instance.description = slingData[1];

					GalaxySoulEnchantment enchant = __instance.GetEnchantmentOfType<GalaxySoulEnchantment>();
					if (enchant != null)					
						__instance.RemoveEnchantment(enchant);
									
				}
				if (count_towards_stats && !enchantment.IsForge())
				{
					__instance.previousEnchantments.Insert(0, enchantment.GetName());
					while (__instance.previousEnchantments.Count > 2)
					{
						__instance.previousEnchantments.RemoveAt(__instance.previousEnchantments.Count - 1);
					}
					Game1.stats.incrementStat("timesEnchanted", 1);
				}
				__result = true;
				return;
			}
			__result = false;
			return;
		}

		[HarmonyPostfix]
		[HarmonyPatch(nameof(Tool.AddEnchantment))]
		public static void AddEnchantment_Postfix(Tool __instance, ref bool __result, BaseEnchantment enchantment)
		{
			if (enchantment != null && __instance is Slingshot && enchantment.IsSecondaryEnchantment())
			{
				var lastUser = AccessTools.FieldRefAccess<Tool, Farmer>("lastUser");

				//__instance.RemoveEnchantment(enchantment);
				__instance.enchantments.Remove(enchantment);
				enchantment.UnapplyTo(__instance, lastUser(__instance));

				foreach (BaseEnchantment existing_enchantment in __instance.enchantments)
				{
					if (enchantment.GetType() == existing_enchantment.GetType())
					{
						if (existing_enchantment.GetMaximumLevel() < 0 || existing_enchantment.GetLevel() < existing_enchantment.GetMaximumLevel())
						{
							existing_enchantment.SetLevel(__instance, existing_enchantment.GetLevel() + 1);
							__result = true;
							return;
						}
						__result = false;
						return;
					}
				}		
				__instance.enchantments.Add(enchantment);
				enchantment.ApplyTo(__instance, lastUser(__instance));
				__result = true;
				return;
			}
			
		}	
		
	}
}
