/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dunc4nNT/StardewMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System;
using xTile.Dimensions;
using Rectangle = xTile.Dimensions.Rectangle;

namespace NeverToxic.StardewMods.SelfServe.Framework
{
    internal class Patches
    {
        private static Harmony s_harmony;
        private static IMonitor s_monitor;
        private static Func<ModConfig> s_config;
        private static IReflectionHelper s_reflectionHelper;

        internal static void Initialise(Harmony harmony, IMonitor monitor, Func<ModConfig> config, IReflectionHelper reflectionHelper)
        {
            s_harmony = harmony;
            s_monitor = monitor;
            s_config = config;
            s_reflectionHelper = reflectionHelper;

            ApplyPatches();
        }

        private static void ApplyPatches()
        {
            s_harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performAction), [typeof(string[]), typeof(Farmer), typeof(Location)]),
                prefix: new HarmonyMethod(typeof(Patches), nameof(GameLocationPerformActionPatch))
            );
            s_harmony.Patch(
               original: AccessTools.Method(typeof(Forest), nameof(Forest.checkAction), [typeof(Location), typeof(Rectangle), typeof(Farmer)]),
               prefix: new HarmonyMethod(typeof(Patches), nameof(ForestCheckActionPatch))
            );
            s_harmony.Patch(
               original: AccessTools.Method(typeof(IslandSouth), nameof(IslandSouth.checkAction), [typeof(Location), typeof(Rectangle), typeof(Farmer)]),
               prefix: new HarmonyMethod(typeof(Patches), nameof(IslandSouthCheckActionPatch))
            );
            s_harmony.Patch(
               original: AccessTools.Method(typeof(Desert), nameof(Desert.OnDesertTrader)),
               prefix: new HarmonyMethod(typeof(Patches), nameof(DesertOnDesertTraderPatch))
            );
            s_harmony.Patch(
               original: AccessTools.Method(typeof(BeachNightMarket), nameof(BeachNightMarket.checkAction)),
               prefix: new HarmonyMethod(typeof(Patches), nameof(BeachNightMarketCheckActionPatch))
            );
        }

        private static bool GameLocationPerformActionPatch(ref GameLocation __instance, string[] action, ref bool __result)
        {
            try
            {
                ModConfig config = s_config();

                if (!ArgUtility.TryGet(action, 0, out string actionType, out string error))
                    return true;

                switch (actionType)
                {
                    case "Buy":
                        if (!ArgUtility.TryGet(action, 1, out string which, out string error2))
                            return true;

                        switch (which)
                        {
                            case "General":
                                if (!config.PierresGeneralShop)
                                    return true;

                                Utility.TryOpenShopMenu(Game1.shop_generalStore, __instance, forceOpen: true);
                                __result = true;
                                return false;
                            case "Fish":
                                if (!config.WillysFishShop)
                                    return true;

                                Utility.TryOpenShopMenu(Game1.shop_fish, __instance, forceOpen: true);
                                __result = true;
                                return false;
                            case "SandyShop":
                                if (!config.SandyOasisShop)
                                    return true;

                                Utility.TryOpenShopMenu(Game1.shop_sandy, __instance, forceOpen: true);
                                __result = true;
                                return false;
                        }
                        return true;

                    case Game1.shop_iceCreamStand:
                        if (!config.IceCreamShop)
                            return true;

                        Utility.TryOpenShopMenu(actionType, __instance, forceOpen: true);
                        __result = true;
                        return false;
                    case Game1.shop_blacksmith:
                        if (!config.BlacksmithShop)
                            return true;

                        ShopHelper.OpenBlacksmithShop(__instance);
                        __result = true;
                        return false;
                    case Game1.shop_carpenter:
                        if (!config.CarpentersShop)
                            return true;

                        ShopHelper.OpenCarpentersShop(__instance);
                        __result = true;
                        return false;
                    case Game1.shop_animalSupplies:
                        if (!config.MarniesAnimalShop)
                            return true;

                        ShopHelper.OpenAnimalSuppliesShop(__instance);
                        __result = true;
                        return false;
                    case "HospitalShop":
                        if (!config.HospitalShop)
                            return true;

                        Utility.TryOpenShopMenu(Game1.shop_hospital, __instance, forceOpen: true);
                        __result = true;
                        return false;
                    case Game1.shop_saloon:
                        if (!config.SaloonShop)
                            return true;

                        Utility.TryOpenShopMenu(actionType, __instance, forceOpen: true);
                        __result = true;
                        return false;
                    case Game1.shop_bookseller:
                        if (!config.BooksellerShop)
                            return true;

                        ShopHelper.OpenBookSellerShop(__instance);
                        __result = true;
                        return false;
                }

                return true;
            }
            catch (Exception e)
            {
                s_monitor.Log($"Failed in {nameof(GameLocationPerformActionPatch)}:\n{e}", LogLevel.Error);
                return true;
            }
        }


        private static bool ForestCheckActionPatch(ref Forest __instance, Location tileLocation, ref bool __result)
        {
            try
            {
                ModConfig config = s_config();

                if (__instance.travelingMerchantDay)
                {
                    Point cartTile = __instance.GetTravelingMerchantCartTile();
                    if (tileLocation.X == cartTile.X + 4 && tileLocation.Y == cartTile.Y + 1 && config.TravelingMerchantShop)
                    {
                        Utility.TryOpenShopMenu(Game1.shop_travelingCart, __instance, forceOpen: true);
                        __result = true;
                        return false;
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                s_monitor.Log($"Failed in {nameof(ForestCheckActionPatch)}:\n{e}", LogLevel.Error);
                return true;
            }
        }

        private static bool IslandSouthCheckActionPatch(ref Forest __instance, Location tileLocation, ref bool __result)
        {
            try
            {
                ModConfig config = s_config();

                if (tileLocation.X == 14 && tileLocation.Y == 22 && config.ResortBarShop)
                {
                    Utility.TryOpenShopMenu(Game1.shop_resortBar, __instance, forceOpen: true);
                    __result = true;
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                s_monitor.Log($"Failed in {nameof(IslandSouthCheckActionPatch)}:\n{e}", LogLevel.Error);
                return true;
            }
        }

        private static bool DesertOnDesertTraderPatch(ref Desert __instance)
        {
            try
            {
                ModConfig config = s_config();

                if (config.DesertTraderShop)
                {
                    Utility.TryOpenShopMenu(Game1.shop_desertTrader, __instance, forceOpen: true);
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                s_monitor.Log($"Failed in {nameof(DesertOnDesertTraderPatch)}:\n{e}", LogLevel.Error);
                return true;
            }
        }

        private static bool BeachNightMarketCheckActionPatch(ref BeachNightMarket __instance, Location tileLocation, ref bool __result)
        {
            try
            {
                ModConfig config = s_config();

                switch (__instance.getTileIndexAt(tileLocation, "Buildings"))
                {
                    case 68:
                        if (!config.NightMarketPainterShop)
                            return true;

                        if (Game1.player.mailReceived.Contains(s_reflectionHelper.GetField<string>(__instance, "paintingMailKey").GetValue()))
                            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_PainterSold"));
                        else
                            __instance.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_PainterQuestion"), __instance.createYesNoResponses(), "PainterQuestion");

                        __result = true;
                        return false;
                    case 70:
                        if (!config.NightMarketMagicBoatShop)
                            return true;

                        Utility.TryOpenShopMenu("Festival_NightMarket_MagicBoat_Day" + __instance.getDayOfNightMarket(), __instance, forceOpen: true);

                        __result = true;
                        return false;
                    case 399:
                        if (!config.NightMarketTravelingMerchantShop)
                            return true;

                        Utility.TryOpenShopMenu(Game1.shop_travelingCart, __instance, forceOpen: true);

                        __result = true;
                        return false;
                    case 595:
                        if (!config.NightMarketDecorationBoatShop)
                            return true;

                        Utility.TryOpenShopMenu("Festival_NightMarket_DecorationBoat", __instance, forceOpen: true);

                        __result = true;
                        return false;
                }

                return true;
            }
            catch (Exception e)
            {
                s_monitor.Log($"Failed in {nameof(BeachNightMarketCheckActionPatch)}:\n{e}", LogLevel.Error);
                return true;
            }
        }
    }
}
