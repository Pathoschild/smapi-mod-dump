/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rusunu/Enchantment
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Tools;
using HarmonyLib;
using StardewValley.Enchantments;

namespace Enchantment
{
    internal class ModEntry : Mod
    {
        internal static IMonitor ModMonitor { get; set; }
        internal new static IModHelper Helper { get; set; }

        public override void Entry(IModHelper helper)
        {
            ModMonitor = Monitor;
            Helper = helper;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }
        private void OnGameLaunched(object sender, EventArgs e)
        {
            // Override Tool & ForgeMenu functionality
            var harmony = new Harmony("Stari.ManyEnchantments");
            harmony.Patch(
                original: AccessTools.Method(typeof(Tool), nameof(Tool.AddEnchantment)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.AddEnchantment_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Tool), nameof(Tool.CanAddEnchantment)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.CanAddEnchantment_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Tool), nameof(Tool.Forge)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.Forge_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Menus.ForgeMenu), nameof(StardewValley.Menus.ForgeMenu.GetForgeCost)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.GetForgeCost_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Menus.ForgeMenu), nameof(StardewValley.Menus.ForgeMenu.IsValidUnforge)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.IsValidUnforge_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Menus.ForgeMenu), nameof(StardewValley.Menus.ForgeMenu.HighlightItems)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.HighlightItems_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Menus.ForgeMenu), nameof(StardewValley.Menus.ForgeMenu.IsValidCraftIngredient)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.IsValidCraftIngredient_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Menus.ForgeMenu), nameof(StardewValley.Menus.ForgeMenu.receiveLeftClick)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.ReceiveLeftClick_Prefix))
            );
        }

        public static List<int> GetValidForgeEnchantmentsForTool(Tool __instance)
        {
            List<int> result = new List<int>();
            if (__instance.GetEnchantmentLevel<EmeraldEnchantment>() < 3)
            {
                result.Add(0);
            }
            if (__instance.GetEnchantmentLevel<AquamarineEnchantment>() < 3)
            {
                result.Add(1);
            }
            if (__instance.GetEnchantmentLevel<RubyEnchantment>() < 3)
            {
                result.Add(2);
            }
            if (__instance.GetEnchantmentLevel<AmethystEnchantment>() < 3)
            {
                result.Add(3);
            }
            if (__instance.GetEnchantmentLevel<TopazEnchantment>() < 3)
            {
                result.Add(4);
            }
            if (__instance.GetEnchantmentLevel<JadeEnchantment>() < 3)
            {
                result.Add(5);
            }
            return result;
        }
        public static bool IsEnchantedOrEnchantable(Item item)
        {
            if (item != null && item is Tool)
            {
                foreach (BaseEnchantment enchantment in (item as Tool).enchantments)
                {
                    if (!enchantment.IsForge() && !enchantment.IsSecondaryEnchantment())
                    {
                        return true;
                    }
                }
                if (BaseEnchantment.GetAvailableEnchantmentsForItem(item as Tool).Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        [HarmonyPrefix]
        public static bool AddEnchantment_Prefix(Tool __instance, BaseEnchantment enchantment, ref bool __result)
        {
            try
            {
                if (enchantment == null)
                {
                    return true;
                }
                if (!enchantment.IsForge() && !enchantment.IsSecondaryEnchantment())
                {
                    // Enchantment is a primary enchantment.
                    __instance.enchantments.Add(enchantment);
                    enchantment.ApplyTo(__instance, __instance.getLastFarmerToUse());
                    __result = true;
                    return false;  // don't run original logic
                }
                if (__instance is MeleeWeapon && enchantment.IsForge())
                {
                    if (enchantment is DiamondEnchantment)
                    {
                        // Skip adding diamond enchantments, they should result in 3 other enchantments getting added in Forge.
                        __result = true;
                        return false;  // don't run original logic
                    }
                    // Enchantment is a Weapon forging or Galaxy Soul enchantment
                }
                return true;  // run original logic
            }
            catch (Exception ex)
            {
                ModMonitor.Log($"Failed in {nameof(AddEnchantment_Prefix)}:\n{ex}", LogLevel.Error);
                return true;  // run original logic
            }
        }
        [HarmonyPrefix]
        public static bool CanAddEnchantment_Prefix(Tool __instance, BaseEnchantment enchantment, ref bool __result)
        {
            try
            {
                if (enchantment == null)
                {
                    return true; // run original logic
                }
                if (__instance is MeleeWeapon && enchantment.IsForge())
                {
                    if (enchantment is DiamondEnchantment && GetValidForgeEnchantmentsForTool(__instance).Count <= 0)
                    {
                        // No more forge enchantments can be added.
                        __result = false;
                        return false;
                    }
                    // Enchantment is a normal forge enchantment, check the existing level if there is one
                    foreach (BaseEnchantment exisiting_enchantment in __instance.enchantments)
                    {
                        if (enchantment.GetType() == exisiting_enchantment.GetType())
                        {
                            if (exisiting_enchantment.GetLevel() >= 3)
                            {
                                __result = false;
                                return false;
                            }
                            break;
                        }
                    }
                    __result = true;
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                ModMonitor.Log($"Failed in {nameof(CanAddEnchantment_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
        [HarmonyPrefix]
        public static bool Forge_Prefix(Tool __instance, Item item, ref bool __result)
        {
            try
            {
                BaseEnchantment enchantment = BaseEnchantment.GetEnchantmentFromItem(__instance, item);
                // This gets displayed twice because the game does it once to display the "Result" item in the menu.
                ModMonitor.Log($"Adding {(enchantment != null ? (enchantment.IsForge() ? "forge enchantment" : enchantment.GetDisplayName()) : "null enchantment")} from item {(item != null ? item.Name : "null")} to tool {__instance.Name}", LogLevel.Debug);
                if (enchantment != null && enchantment is DiamondEnchantment)
                {
                    // Diamond Enchantment is now replaced with up to 3 other random enchantments. Still get a slight discount on the cost.
                    if (GetValidForgeEnchantmentsForTool(__instance).Count <= 0)
                    {
                        __result = false;
                        return false;
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        // Try to add 3 new enchantments.
                        List<int> valid_forges = GetValidForgeEnchantmentsForTool(__instance);
                        if (valid_forges.Count <= 0)
                        {
                            // Can't add enchantments
                            break;
                        }
                        int index = Game1.random.Next(valid_forges.Count);
                        int random_enchant = valid_forges[index];
                        switch (random_enchant)
                        {
                            case 0:
                                ModMonitor.Log($"Adding Emerald Enchantment as part of Diamond Enchantment process", LogLevel.Trace);
                                __instance.AddEnchantment(new EmeraldEnchantment());
                                break;
                            case 1:
                                ModMonitor.Log($"Adding Emerald Aquamarine as part of Diamond Enchantment process", LogLevel.Trace);
                                __instance.AddEnchantment(new AquamarineEnchantment());
                                break;
                            case 2:
                                ModMonitor.Log($"Adding Ruby Enchantment as part of Diamond Enchantment process", LogLevel.Trace);
                                __instance.AddEnchantment(new RubyEnchantment());
                                break;
                            case 3:
                                ModMonitor.Log($"Adding Amethyst Enchantment as part of Diamond Enchantment process", LogLevel.Trace);
                                __instance.AddEnchantment(new AmethystEnchantment());
                                break;
                            case 4:
                                ModMonitor.Log($"Adding Topaz Enchantment as part of Diamond Enchantment process", LogLevel.Trace);
                                __instance.AddEnchantment(new TopazEnchantment());
                                break;
                            case 5:
                                ModMonitor.Log($"Adding Jade Enchantment as part of Diamond Enchantment process", LogLevel.Trace);
                                __instance.AddEnchantment(new JadeEnchantment());
                                break;
                        }
                    }
                    __result = true;
                    return false; // don't run original logic
                }
                return true; // run original logic
            }
            catch (Exception ex)
            {
                ModMonitor.Log($"Failed in {nameof(Forge_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
        [HarmonyPrefix]
        public static bool GetForgeCost_Prefix(StardewValley.Menus.ForgeMenu __instance, Item left_item, Item right_item, ref int __result)
        {
            try
            {
                if (right_item != null && right_item.QualifiedItemId == "(O)72") //old: Utility.IsNormalObjectAtParentSheetIndex(right_item, 72)
                {

                    if (left_item != null && left_item is Tool)
                    {
                        __result = __instance.GetForgeCostAtLevel((left_item as Tool).GetTotalForgeLevels()) * 3;
                    }
                    else
                    {
                        __result = 30;
                    }
                    return false;  // don't run original logic
                }
                return true;  // run original logic
            }
            catch (Exception ex)
            {
                ModMonitor.Log($"Failed in {nameof(GetForgeCost_Prefix)}:\n{ex}", LogLevel.Error);
                return true;  // run original logic
            }
        }
        [HarmonyPrefix]
        public static bool IsValidUnforge_Prefix(StardewValley.Menus.ForgeMenu __instance, ref bool __result)
        {
            try
            {
                if (__instance.rightIngredientSpot.item == null && __instance.leftIngredientSpot.item != null && __instance.leftIngredientSpot.item is MeleeWeapon)
                {
                    // If enchanted, also is valid to unforge.
                    foreach (BaseEnchantment enchantment in (__instance.leftIngredientSpot.item as MeleeWeapon).enchantments)
                    {
                        if (!enchantment.IsForge() && !enchantment.IsSecondaryEnchantment())
                        {
                            __result = true;
                            return false;  // don't run original logic
                        }
                    }
                }
                return true;  // run original logic
            }
            catch (Exception ex)
            {
                ModMonitor.Log($"Failed in {nameof(IsValidUnforge_Prefix)}:\n{ex}", LogLevel.Error);
                return true;  // run original logic
            }
        }
        [HarmonyPrefix]
        public static bool HighlightItems_Prefix(StardewValley.Menus.ForgeMenu __instance, Item i, ref bool __result)
        {
            // Override highlight items for the case when the tool is enchanted fully. Base game assumes that a tool will always be enchantable, as every tool has more than one enchantment.
            try
            {
                if (IsEnchantedOrEnchantable(i))
                {
                    __result = true;
                    return false;  // don't run original logic
                }
                return true;  // run original logic
            }
            catch (Exception ex)
            {
                ModMonitor.Log($"Failed in {nameof(HighlightItems_Prefix)}:\n{ex}", LogLevel.Error);
                return true;  // run original logic
            }
        }
        [HarmonyPrefix]
        public static bool IsValidCraftIngredient_Prefix(StardewValley.Menus.ForgeMenu __instance, Item item, ref bool __result)
        {
            // Override is valid ingredient for the case when the tool is enchanted fully. Base game assumes that a tool will always be enchantable, as every tool has more than one enchantment.
            try
            {
                if (IsEnchantedOrEnchantable(item))
                {
                    __result = true;
                    return false;  // don't run original logic
                }
                return true;  // run original logic
            }
            catch (Exception ex)
            {
                ModMonitor.Log($"Failed in {nameof(IsValidCraftIngredient_Prefix)}:\n{ex}", LogLevel.Error);
                return true;  // run original logic
            }
        }
        [HarmonyPrefix]
        public static bool ReceiveLeftClick_Prefix(StardewValley.Menus.ForgeMenu __instance, int x, int y)
        {
            try
            {
                if (__instance.unforgeButton.containsPoint(x, y) &&
                    __instance.rightIngredientSpot.item == null &&
                    __instance.IsValidUnforge())
                {
                    if (__instance.leftIngredientSpot.item is MeleeWeapon)
                    {
                        MeleeWeapon weapon = __instance.leftIngredientSpot.item as MeleeWeapon;

                        bool has_enchantment = false;
                        foreach (BaseEnchantment enchantment in weapon.enchantments)
                        {
                            if (!enchantment.IsForge() && !enchantment.IsSecondaryEnchantment())
                            {
                                has_enchantment = true;
                                break;
                            }
                        }
                        if (!has_enchantment)
                        {
                            // No enchantments to remove, run original logic to mop up gem forges.
                            return true;  // run original logic
                        }
                        ModMonitor.Log($"Removing enchantments as part of unforging", LogLevel.Debug);
                        for (int i = weapon.enchantments.Count - 1; i >= 0; i--)
                        {
                            if (!weapon.enchantments[i].IsForge() && !weapon.enchantments[i].IsSecondaryEnchantment())
                            {
                                ModMonitor.Log($"Removing {weapon.enchantments[i].GetDisplayName()} as part of unforging", LogLevel.Trace);
                                weapon.RemoveEnchantment(weapon.enchantments[i]);
                            }
                        }
                        weapon.previousEnchantments.Clear();
                        if (weapon.enchantments.Count <= 0)
                        {
                            // No enchantments to remove, don't run original logic to mop up gem forges.
                            Game1.playSound("debuffHit");
                            return false;  // don't run original logic
                        }
                    }
                    else if (__instance.leftIngredientSpot.item is Tool)
                    {
                        Tool tool = __instance.leftIngredientSpot.item as Tool;
                        bool has_enchantment = false;
                        foreach (BaseEnchantment enchantment in tool.enchantments)
                        {
                            if (!enchantment.IsForge() && !enchantment.IsSecondaryEnchantment())
                            {
                                has_enchantment = true;
                                break;
                            }
                        }
                        if (!has_enchantment)
                        {
                            // No enchantments to remove, run original logic to mop up gem forges.
                            return true;  // run original logic
                        }
                        ModMonitor.Log($"Removing enchantments as part of unforging", LogLevel.Debug);
                        for (int i = tool.enchantments.Count - 1; i >= 0; i--)
                        {
                            if (!tool.enchantments[i].IsForge() && !tool.enchantments[i].IsSecondaryEnchantment())
                            {
                                ModMonitor.Log($"Removing {tool.enchantments[i].GetDisplayName()} as part of unforging", LogLevel.Trace);
                                tool.RemoveEnchantment(tool.enchantments[i]);
                            }
                        }
                        tool.previousEnchantments.Clear();
                        Game1.playSound("debuffHit");
                        return false;  // don't run original logic
                    }
                }
                return true;  // run original logic
            }
            catch (Exception ex)
            {
                ModMonitor.Log($"Failed in {nameof(ReceiveLeftClick_Prefix)}:\n{ex}", LogLevel.Error);
                return true;  // run original logic
            }

        }
    }
}
