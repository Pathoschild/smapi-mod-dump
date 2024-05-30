/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SlivaStari/CombineManyRings
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using StardewValley.Menus;
using GenericModConfigMenu;

namespace CombineManyRings
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        internal static IMonitor ModMonitor { get; set; }
        internal new static IModHelper Helper { get; set; }

        internal static ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            ModMonitor = Monitor;
            Helper = helper;

            Config = helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private void OnGameLaunched(object sender, EventArgs e)
        {
            Harmony harmony = new Harmony("Stari.CombineManyRings");

            harmony.Patch(
                original: AccessTools.Method(typeof(Ring), nameof(Ring.CanCombine)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.CanCombine_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(CombinedRing), "loadDisplayFields"),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.LoadDisplayFields_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(CombinedRing), nameof(CombinedRing.drawInMenu), new Type[] {typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool)}),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.DrawInMenu_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(ForgeMenu), nameof(ForgeMenu.GetForgeCost)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.GetForgeCost_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(ForgeMenu), nameof(ForgeMenu.CraftItem)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.CraftItem_Prefix))
            );
            //public void SpendRightItem ();
            harmony.Patch(
                original: AccessTools.Method(typeof(ForgeMenu), nameof(ForgeMenu.SpendRightItem)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.SpendRightItem_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(ForgeMenu), "_UpdateDescriptionText"),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry._UpdateDescriptionText_Postfix))
            );

            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod in GMCM
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Allow Combining the Same Ring",
                tooltip: () => "If true, allow combining the same ring into a combined ring (including multiple times). Very Overpowered.",
                getValue: () => Config.AllowSameRing,
                setValue: value => Config.AllowSameRing = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Balanced Combination Mode",
                tooltip: () => "If true, combining rings will cost escalating amounts of shards and/or risk of breaking.",
                getValue: () => Config.BalancedMode,
                setValue: value => Config.BalancedMode = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Destroy Ring on Failure",
                tooltip: () => "If true, combining rings has a chance of failure that destroys a ring. Requires Balanced Combination Mode.",
                getValue: () => Config.DestroyRingOnFailure,
                setValue: value => Config.DestroyRingOnFailure = value
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Failure Chance Per Extra Ring",
                tooltip: () => "Chance of Failure per extra ring combined, capped at a maximum of 90% failure rate. Requires Balanced Combination Mode.",
                getValue: () => Config.FailureChancePerExtraRing,
                setValue: value => Config.FailureChancePerExtraRing = value
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Cost per extra ring",
                tooltip: () => "Cost per extra ring, capped at 999. Requires Balanced Combination Mode.",
                getValue: () => Config.CostPerExtraRing,
                setValue: value => Config.CostPerExtraRing = value
            );
        }
        public static int GetCombinedRingTotal(Ring ring)
        {
            if (ring is CombinedRing)
            {
                int count = 0;
                foreach (Ring r in (ring as CombinedRing).combinedRings)
                {
                    count += GetCombinedRingTotal(r);
                }
                return count;
            }
            else
            {
                return 1;
            }
        }
        public static SortedDictionary<string, int> GetCombinedRings(Ring ring)
        {
            SortedDictionary<string, int> result = new SortedDictionary<string, int>();
            Queue<Ring> to_process = new Queue<Ring>();
            to_process.Enqueue(ring);
            while (to_process.Count > 0) {
                Ring cur = to_process.Dequeue();
                ModMonitor.Log($"Processing {cur.DisplayName}", LogLevel.Trace);
                if (cur is CombinedRing)
                {
                    foreach(Ring r in (cur as CombinedRing).combinedRings)
                    {
                        to_process.Enqueue(r);
                    }
                }
                else
                {
                    string key = cur.displayName;
                    if (result.TryGetValue(key, out int val))
                    {
                        result[key] = val + 1;
                    }
                    else
                    {
                        result.Add(key, 1);
                    }
                }
            }
            return result;
        }
        public static void LoadDisplayFields_Postfix(CombinedRing __instance)
        {
            try
            {
                // If the total is less than 8, the each ring's description is included. If more, then only ring name is displayed for brevity.
                if (GetCombinedRingTotal(__instance) >= 8)
                {
                    string description = "Many Rings forged into one:\n\n";
                    foreach (KeyValuePair<string, int> entry in GetCombinedRings(__instance))
                    {
                        // Only one ring of each type is normally allowed, unless Config.AllowSameRing is set.
                        description += String.Format("{1}x {0}\n", entry.Key, entry.Value);
                    }
                    ModMonitor.Log($"Combined Ring description is {description}", LogLevel.Trace);
                    __instance.description = description.Trim();
                }
            }
            catch (Exception ex)
            {
                ModMonitor.Log($"Failed in {nameof(LoadDisplayFields_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }
        public static bool CanCombine_Prefix(Ring __instance, Ring ring, ref bool __result)
        {
            try
            {
                __result = true;
                if (Config.AllowSameRing)
                {
                    // Always return true;
                    return false;
                }
                    if (ring is CombinedRing)
                {
                    foreach (Ring combinedRing in (ring as CombinedRing).combinedRings)
                    {
                        if (!__instance.CanCombine(combinedRing))
                        {
                            __result = false;
                            break;
                        }
                    }
                }
                else if (__instance is CombinedRing)
                {
                    foreach (Ring combinedRing in (__instance as CombinedRing).combinedRings)
                    {
                        if (!combinedRing.CanCombine(ring))
                        {
                            __result = false;
                            break;
                        }
                    }
                }
                else if (__instance.ParentSheetIndex == ring.ParentSheetIndex)
                {
                    __result = false;
                }
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                ModMonitor.Log($"Failed in {nameof(CanCombine_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
        public static bool DrawInMenu_Prefix(CombinedRing __instance, SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            try
            {
                if (__instance.combinedRings.Count >= 2)
                {
                    // Always use base rings as the sprites to draw. The first pair that are combined on the left hand side get used as the sprite.
                    if (__instance.combinedRings[0] is CombinedRing)
                    {
                        __instance.combinedRings[0].drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
                        return false; // don't run original logic
                    }
                    else if (__instance.combinedRings[1] is CombinedRing)
                    {
                        __instance.combinedRings[1].drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
                        return false; // don't run original logic
                    }
                }
                return true; // run original logic
            }
            catch (Exception ex)
            {
                ModMonitor.Log($"Failed in {nameof(DrawInMenu_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
        public static void GetForgeCost_Postfix(ForgeMenu __instance, Item left_item, Item right_item, ref int __result)
        {
            try
            {
                if (!Config.BalancedMode)
                {
                    // No changes needed, allow default behavior
                    return;
                }
                if (left_item != null && right_item != null)
                {
                    // if merging rings, then calculate different cost based on the total amount of rings being combined
                    // if only two, rings, than keep normal cost of 20, otherwise, gets 100 per ring combined (max of 999)
                    if (left_item.getCategoryName().Equals("Ring") && left_item.Category == right_item.Category)
                    {
                        Ring left_ring = (Ring)left_item;
                        Ring right_ring = (Ring)right_item;

                        int total_rings = GetCombinedRingTotal(left_ring) + GetCombinedRingTotal(right_ring);
                        if (total_rings > 2)
                        {
                            int new_cost = GetTotalCombinedRingsCost(total_rings);
                            __result = new_cost;

                        }

                    }

                }

            }
            catch (Exception ex)
            {
                ModMonitor.Log($"Failed in {nameof(GetForgeCost_Postfix)}:\n{ex}", LogLevel.Error);
            }

        }


        public static bool CraftItem_Prefix(ForgeMenu __instance, ref ForgeMenu.CraftState ____craftState, string ___displayedDescription, Item left_item, Item right_item, ref Item __result, bool forReal = false)
        {
            try
            {
                if (!Config.BalancedMode) {
                    // No changes needed, allow default behavior
                    return true;
                }
                if (left_item != null && right_item != null)
                {
                    if (left_item.getCategoryName().Equals("Ring") && left_item.Category == right_item.Category)
                    {
                        Ring left_ring = (Ring)left_item;
                        Ring right_ring = (Ring)right_item;

                        int total_left_rings = GetCombinedRingTotal(left_ring);
                        int total_right_rings = GetCombinedRingTotal(right_ring);
                        int total_rings = total_left_rings + total_right_rings;
                        if (forReal == true)
                        {
                            int breakChance = GetBreakChance(total_rings);
                            Random r = new Random();
                            int instabilityForgeRoll = r.Next(0, 100);
                            if (instabilityForgeRoll < breakChance)
                            {
                                ModMonitor.Log($"Was unstable ({instabilityForgeRoll} of {breakChance}). Will not forge the result", LogLevel.Trace);

                                if (Config.DestroyRingOnFailure)
                                {

                                    int brokenRingRoll = r.Next(0, total_rings);
                                    bool keepRightItem = true;

                                    if (total_left_rings >= total_right_rings && brokenRingRoll >= total_left_rings)
                                    {
                                        keepRightItem = false;
                                    }
                                    else if (total_left_rings < total_right_rings && brokenRingRoll < total_right_rings)
                                    {
                                        keepRightItem = false;
                                    }
                                    if (keepRightItem)
                                    {
                                        __result = right_item;
                                        __instance.leftIngredientSpot.item = right_item;
                                        __instance.rightIngredientSpot.item = left_item;

                                        ModMonitor.Log($"keeping right ring", LogLevel.Trace);
                                    }
                                    else
                                    {
                                        __result = left_item;
                                        ModMonitor.Log($"keeping left ring", LogLevel.Trace);
                                    }
                                    ModMonitor.Log($"brokenRingRoll: {brokenRingRoll}, {total_left_rings}, {total_right_rings}", LogLevel.Trace);

                                }
                                else
                                {
                                    __result = left_ring;
                                    ____craftState = ForgeMenu.CraftState.InvalidRecipe;
                                }
                                Game1.playSound("rockGolemDie");
                                return false;
                            }
                            else
                            {
                                ModMonitor.Log($"Successfull {instabilityForgeRoll} of {breakChance}! will forge as it should.", LogLevel.Trace);
                            }
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                ModMonitor.Log($"Failed in {nameof(CraftItem_Prefix)}:\n{ex}", LogLevel.Error);
            }


            ModMonitor.Log($"running original", LogLevel.Trace);
            return true;
        }

        public static bool SpendRightItem_Prefix(ref ForgeMenu.CraftState ____craftState)
        {
            try
            {
                if (!Config.BalancedMode)
                {
                    // No changes needed, allow default behavior
                    return true;
                }
                if (____craftState == ForgeMenu.CraftState.InvalidRecipe)
                {
                    // return state to the original one, just to avoid messing up anymore than we already are.
                    ____craftState = ForgeMenu.CraftState.Valid;

                    ModMonitor.Log($"Was an unstable forge, but wont consume the right item.", LogLevel.Trace);
                    return false;
                }

            }
            catch (Exception ex)
            {
                ModMonitor.Log($"Failed in {nameof(SpendRightItem_Prefix)}:\n{ex}", LogLevel.Error);
            }
            ModMonitor.Log($"running Original SpendRightItem", LogLevel.Trace);
            return true;

        }
        public static int GetBreakChance(int total_rings)
        {
            return Math.Min((total_rings - 2) * Config.FailureChancePerExtraRing, 90);  // 0 - 90
        }

        public static int GetTotalCombinedRingsCost(int total_rings)
        {
            return Math.Min((total_rings - 2) * Config.CostPerExtraRing, 999);
        }

        public static void _UpdateDescriptionText_Postfix(ForgeMenu __instance, ref string ___displayedDescription)
        {
            try
            {
                if (!Config.BalancedMode)
                {
                    // No changes needed, allow default behavior
                    return;
                }
                if (__instance.inventory != null && __instance.leftIngredientSpot != null && __instance.rightIngredientSpot != null)
                {
                    Item left_item = __instance.leftIngredientSpot.item;
                    Item right_item = __instance.rightIngredientSpot.item;



                    if (left_item != null && right_item != null)
                    {

                        if (left_item.getCategoryName().Equals("Ring") && left_item.Category == right_item.Category)
                        {
                            Ring left_ring = (Ring)left_item;
                            Ring right_ring = (Ring)right_item;

                            int total_left_rings = GetCombinedRingTotal(left_ring);
                            int total_right_rings = GetCombinedRingTotal(right_ring);
                            int total_rings = total_left_rings + total_right_rings;
                            int cost = GetTotalCombinedRingsCost(total_rings);
                            if (total_rings > 2)
                            {
                                ___displayedDescription += $"\nCost: {cost}";
                            }
                            int sucess_rate = 100 - GetBreakChance(total_rings);
                            ___displayedDescription += $"\nChance of sucess: {sucess_rate}%";

                        }
                    }


                }
            }
            catch (Exception ex)
            {
                ModMonitor.Log($"Failed in {nameof(_UpdateDescriptionText_Postfix)}:\n{ex}", LogLevel.Error);
            }

        }
    }
}