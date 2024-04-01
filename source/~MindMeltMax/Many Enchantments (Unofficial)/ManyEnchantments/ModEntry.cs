/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Enchantments;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManyEnchantments
{
    internal class ModEntry : Mod
    {
        internal static IMonitor IMonitor;

        public override void Entry(IModHelper helper)
        {
            IMonitor = Monitor;

            helper.Events.GameLoop.GameLaunched += onGameLaunched;
        }

        private void onGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            Harmony harmony = new(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(Tool), nameof(Tool.AddEnchantment)),
                prefix: new(typeof(ModEntry), nameof(AddEnchantmentPrefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Tool), nameof(Tool.CanAddEnchantment)),
                prefix: new(typeof(ModEntry), nameof(CanAddEnchantmentPrefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Tool), nameof(Tool.Forge)),
                prefix: new(typeof(ModEntry), nameof(ForgePrefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(ForgeMenu), nameof(ForgeMenu.GetForgeCost)),
                prefix: new(typeof(ModEntry), nameof(GetForgeCostPrefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(ForgeMenu), nameof(ForgeMenu.IsValidUnforge)),
                prefix: new(typeof(ModEntry), nameof(IsValidUnforgePrefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(ForgeMenu), nameof(ForgeMenu.HighlightItems)),
                prefix: new(typeof(ModEntry), nameof(HighlightItemsPrefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(ForgeMenu), nameof(ForgeMenu.IsValidCraftIngredient)),
                prefix: new(typeof(ModEntry), nameof(IsValidCraftIngredientPrefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(ForgeMenu), nameof(ForgeMenu.receiveLeftClick)),
                prefix: new(typeof(ModEntry), nameof(ReceiveLeftClickPrefix))
            );
        }

        public static List<int> GetValidForgeEnchantmentsForTool(Tool t)
        {
            List<int> result = new();
            if (t.GetEnchantmentLevel<EmeraldEnchantment>() < 3)
                result.Add(0);
            if (t.GetEnchantmentLevel<AquamarineEnchantment>() < 3)
                result.Add(1);
            if (t.GetEnchantmentLevel<RubyEnchantment>() < 3)
                result.Add(2);
            if (t.GetEnchantmentLevel<AmethystEnchantment>() < 3)
                result.Add(3);
            if (t.GetEnchantmentLevel<TopazEnchantment>() < 3)
                result.Add(4);
            if (t.GetEnchantmentLevel<JadeEnchantment>() < 3)
                result.Add(5);
            return result;
        }
        public static bool IsEnchantedOrEnchantable(Item item)
        {
            if (item is not Tool t)
                return false;
            foreach (var enchant in t.enchantments)
                if (!enchant.IsForge() && !enchant.IsSecondaryEnchantment())
                    return true;
            if (BaseEnchantment.GetAvailableEnchantmentsForItem(t).Count > 0)
                return true;
            return false;
        }

        private static bool AddEnchantmentPrefix(Tool __instance, BaseEnchantment enchantment, ref bool __result)
        {
            try
            {
                if (enchantment is null)
                    return true;
                if (!enchantment.IsForge() && !enchantment.IsSecondaryEnchantment())
                {
                    __instance.enchantments.Add(enchantment);
                    enchantment.ApplyTo(__instance, __instance.getLastFarmerToUse());
                    __result = true;
                    return false;
                }
                if (__instance is MeleeWeapon && enchantment.IsForge() && enchantment is DiamondEnchantment)
                {
                    __result = true;
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                IMonitor.Log($"Failed patching {nameof(Tool.AddEnchantment)}", LogLevel.Error);
                IMonitor.Log($"{ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
                return true;
            }
        }

        private static bool CanAddEnchantmentPrefix(Tool __instance, BaseEnchantment enchantment, ref bool __result)
        {
            try
            {
                if (enchantment is null)
                    return true;
                if (__instance is MeleeWeapon && enchantment.IsForge())
                {
                    if (enchantment is DiamondEnchantment && GetValidForgeEnchantmentsForTool(__instance).Count <= 0)
                    {
                        __result = false;
                        return false;
                    }
                    foreach (var enchant in __instance.enchantments)
                    {
                        if (enchant.GetType() == enchantment.GetType())
                        {
                            if (enchant.GetLevel() >= 3)
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
                IMonitor.Log($"Failed patching {nameof(Tool.CanAddEnchantment)}", LogLevel.Error);
                IMonitor.Log($"{ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
                return true;
            }
        }

        private static bool ForgePrefix(Tool __instance, Item item, bool count_towards_stats, ref bool __result)
        {
            try
            {
                var enchantment = BaseEnchantment.GetEnchantmentFromItem(__instance, item);
                if (count_towards_stats)
                    IMonitor.Log($"Adding {(enchantment != null ? (enchantment.IsForge() ? "forge enchantment" : enchantment.GetDisplayName()) : "null enchantment")} from item {(item != null ? item.Name : "null")} to tool {__instance.Name}", LogLevel.Debug);
                if (enchantment is DiamondEnchantment)
                {
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
                            break;
                        int index = Game1.random.Next(valid_forges.Count);
                        int random_enchant = valid_forges[index];
                        switch (random_enchant)
                        {
                            case 0:
                                IMonitor.Log($"Adding Emerald Enchantment as part of Diamond Enchantment process", LogLevel.Trace);
                                __instance.AddEnchantment(new EmeraldEnchantment());
                                break;
                            case 1:
                                IMonitor.Log($"Adding Emerald Aquamarine as part of Diamond Enchantment process", LogLevel.Trace);
                                __instance.AddEnchantment(new AquamarineEnchantment());
                                break;
                            case 2:
                                IMonitor.Log($"Adding Ruby Enchantment as part of Diamond Enchantment process", LogLevel.Trace);
                                __instance.AddEnchantment(new RubyEnchantment());
                                break;
                            case 3:
                                IMonitor.Log($"Adding Amethyst Enchantment as part of Diamond Enchantment process", LogLevel.Trace);
                                __instance.AddEnchantment(new AmethystEnchantment());
                                break;
                            case 4:
                                IMonitor.Log($"Adding Topaz Enchantment as part of Diamond Enchantment process", LogLevel.Trace);
                                __instance.AddEnchantment(new TopazEnchantment());
                                break;
                            case 5:
                                IMonitor.Log($"Adding Jade Enchantment as part of Diamond Enchantment process", LogLevel.Trace);
                                __instance.AddEnchantment(new JadeEnchantment());
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
                IMonitor.Log($"Failed patching {nameof(Tool.Forge)}", LogLevel.Error);
                IMonitor.Log($"{ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
                return true;
            }
        }

        private static bool GetForgeCostPrefix(ForgeMenu __instance, Item left_item, Item right_item, ref int __result)
        {
            try
            {
                if (right_item is not null && right_item.QualifiedItemId == "(O)72")
                {
                    if (left_item is Tool t)
                        __result = __instance.GetForgeCostAtLevel(t.GetTotalForgeLevels()) * 3;
                    else
                        __result = 30;
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                IMonitor.Log($"Failed patching {nameof(ForgeMenu.GetForgeCost)}", LogLevel.Error);
                IMonitor.Log($"{ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
                return true;
            }
        }

        private static bool IsValidUnforgePrefix(ForgeMenu __instance, ref bool __result)
        {
            try
            {
                if (__instance.rightIngredientSpot.item is null && __instance.leftIngredientSpot.item is Tool t)
                {
                    foreach (var enchant in t.enchantments)
                    {
                        if (!enchant.IsForge() && !enchant.IsSecondaryEnchantment())
                        {
                            __result = true;
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                IMonitor.Log($"Failed patching {nameof(ForgeMenu.IsValidUnforge)}", LogLevel.Error);
                IMonitor.Log($"{ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
                return true;
            }
        }

        private static bool HighlightItemsPrefix(Item i, ref bool __result)
        {
            try
            {
                if (IsEnchantedOrEnchantable(i))
                {
                    __result = true;
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                IMonitor.Log($"Failed patching {nameof(ForgeMenu.HighlightItems)}", LogLevel.Error);
                IMonitor.Log($"{ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
                return true;
            }
        }

        private static bool IsValidCraftIngredientPrefix(Item item, ref bool __result)
        {
            try
            {
                if (IsEnchantedOrEnchantable(item))
                {
                    __result = true;
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                IMonitor.Log($"Failed patching {nameof(ForgeMenu.IsValidCraftIngredient)}", LogLevel.Error);
                IMonitor.Log($"{ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
                return true;
            }
        }

        private static bool ReceiveLeftClickPrefix(ForgeMenu __instance, int x, int y)
        {
            try
            {
                if (!__instance.unforgeButton.containsPoint(x, y) || __instance.rightIngredientSpot.item is not null || !__instance.IsValidUnforge())
                    return true;
                if (__instance.leftIngredientSpot.item is Tool t)
                {
                    if (Game1.player.couldInventoryAcceptThisItem("(O)848", t.GetTotalForgeLevels() * 5 + (t.GetTotalForgeLevels() - 1) * 2))
                    {
                        bool has_enchantment = false;
                        foreach (BaseEnchantment enchantment in t.enchantments)
                        {
                            if (!enchantment.IsForge() && !enchantment.IsSecondaryEnchantment())
                            {
                                has_enchantment = true;
                                break;
                            }
                        }
                        if (!has_enchantment)
                            return true;
                        IMonitor.Log($"Removing enchantments as part of unforging", LogLevel.Trace);
                        for (int i = t.enchantments.Count - 1; i >= 0; i--)
                        {
                            if (!t.enchantments[i].IsForge() && !t.enchantments[i].IsSecondaryEnchantment())
                            {
                                IMonitor.Log($"Removing {t.enchantments[i].GetDisplayName()} as part of unforging", LogLevel.Trace);
                                t.RemoveEnchantment(t.enchantments[i]);
                            }
                        }
                        t.previousEnchantments.Clear();
                        if (t.enchantments.Count <= 0)
                        {
                            Game1.playSound("debuffHit");
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                IMonitor.Log($"Failed patching {nameof(ForgeMenu.receiveLeftClick)}", LogLevel.Error);
                IMonitor.Log($"{ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
                return true;
            }
        }
    }
}
