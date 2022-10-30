/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using BirbShared;

namespace RanchingToolUpgrades
{
    internal interface HarmonyPatches
    {
        public static void Patch(string id)
        {
            Harmony harmony = new(id);
            try
            {
                // Patch relevent shop inventories, and upgrade actions
                harmony.Patch(
                    original: AccessTools.Method(typeof(Utility), nameof(Utility.getBlacksmithUpgradeStock)),
                    postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Utility_GetBlacksmithUpgradeStock_Postfix)));
                harmony.Patch(
                    original: AccessTools.Method(typeof(Farmer), nameof(Farmer.showHoldingItem)),
                    prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Farmer_ShowHoldingItem_Prefix)));
                harmony.Patch(
                    original: AccessTools.Method(typeof(Utility), nameof(Utility.getAnimalShopStock)),
                    postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Utility_GetAnimalShopStock_Postfix)));
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        /// <summary>
        /// Tries to add cooking tool to Blacksmith shop stock.
        /// </summary>
        public static void Utility_GetBlacksmithUpgradeStock_Postfix(
            Dictionary<ISalable, int[]> __result,
            Farmer who)
        {
            try
            {
                UpgradeablePail.AddToShopStock(itemPriceAndStock: __result, who: who);
                UpgradeableShears.AddToShopStock(itemPriceAndStock: __result, who: who);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed in {nameof(Utility_GetBlacksmithUpgradeStock_Postfix)}\n{ex}");
            }
        }

        /// <summary>
        /// Draws the correct tool sprite when receiving an upgrade.
        /// </summary>
        public static bool Farmer_ShowHoldingItem_Prefix(
            Farmer who)
        {
            try
            {
                
                Item mrg = who.mostRecentlyGrabbedItem;
                if (mrg is UpgradeablePail || mrg is UpgradeableShears)
                {
                    Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(
                        textureName: ModEntry.Assets.SpritesPath,
                        sourceRect: ((ICustomIcon)mrg).IconSource(),
                        animationInterval: 2500f,
                        animationLength: 1,
                        numberOfLoops: 0,
                        position: who.Position + new Vector2(0f, -124f),
                        flicker: false,
                        flipped: false,
                        layerDepth: 1f,
                        alphaFade: 0f,
                        color: Color.White,
                        scale: 4f,
                        scaleChange: 0f,
                        rotation: 0f,
                        rotationChange: 0f)
                    {
                        motion = new Vector2(0f, -0.1f)
                    });
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed in {nameof(Farmer_ShowHoldingItem_Prefix)}\n{ex}");
            }
            return true;
        }

        public static void Utility_GetAnimalShopStock_Postfix(
            Dictionary<ISalable, int[]> __result
            )
        {
            try
            {
                // Keying off of `new MilkPail()` or `new Shears()` doesn't work.
                // Iterate over items for sale, and remove any by the name "Milk Pail" or "Shears"
                foreach (ISalable key in __result.Keys)
                {
                    if (key.Name.Equals("Milk Pail") || key.Name.Equals("Shears"))
                    {
                        __result.Remove(key);
                    }
                }
                if (Game1.player.hasItemWithNameThatContains("Pail") == null)
                {
                    __result.Add(new UpgradeablePail(0), new int[2] { ModEntry.Config.PailBuyCost, 1 });
                }
                if (Game1.player.hasItemWithNameThatContains("Shears") == null)
                {
                    __result.Add(new UpgradeableShears(0), new int[2] { ModEntry.Config.ShearsBuyCost, 1 });
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed in {nameof(Utility_GetAnimalShopStock_Postfix)}\n{ex}");
            }
        }
    }
}
