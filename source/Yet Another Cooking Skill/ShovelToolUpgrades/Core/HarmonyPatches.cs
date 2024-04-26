/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using MoonShared;
using System.Reflection;
using SpaceCore.Interface;
using System.Reflection.Emit;
using StardewValley.Tools;
using StardewValley.Menus;
using StardewValley.Objects;
using GrowableGiantCrops.Framework;
using Microsoft.Xna.Framework.Graphics;
using static StardewValley.Menus.CharacterCustomization;

namespace ShovelToolUpgrades
{
    [HarmonyPatch(typeof(Utility), nameof(Utility.getBlacksmithUpgradeStock))]
    class Utility_GetBlacksmithUpgradeStock
    {
        public static void Postfix(
            Dictionary<ISalable, int[]> __result,
            Farmer who)
        {
            try
            {
                UpgradeableShovel.AddToShopStock(itemPriceAndStock: __result, who: who);
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
        }
    }

    [HarmonyPatch(typeof(Tool), "get_" + nameof(Tool.Name))]
    public static class ToolNamePatch
    {
        public static void Postfix(Tool __instance, ref string __result)
        {
            if (__instance.UpgradeLevel >= 5)
            {
                string tier = __instance.UpgradeLevel == 5 ? "radioactive" : "mythicite";
                string tool = "";
                switch (__instance.BaseName)
                {
                    case "Axe": tool = "axe"; break;
                    case "Watering Can": tool = "wcan"; break;
                    case "Pickaxe": tool = "pick"; break;
                    case "Hoe": tool = "hoe"; break;
                    case "Shovel": tool = "shovel"; break;
                    case "Shears": tool = "shears"; break;
                    case "Ore Pan": tool = "pan"; break;
                    case "Milk Pail": tool = "pail"; break;
                }
                __result = ModEntry.Instance.I18n.Get($"tool."+tool+"."+tier);
            }
        }
    }

    [HarmonyPatch(typeof(SpaceCore.Interface.NewForgeMenu), nameof(SpaceCore.Interface.NewForgeMenu.CraftItem))]
    class NewForgeMenuCraftItemPatcher
    {
        public static void Postfix(ref Item __result, Item left_item, Item right_item)
        {
            ///Make sure margo is loaded to do forge upgrading
            if (ModEntry.MargoLoaded)
            {
                ///If players have the Margo Compact turned off, do not do anything
                if (ModEntry.Config.MargoCompact == false)
                {
                    return;
                }
                ///Check to see if the item is a shovel or upgradeable shovel. If it is not, skip this code
                if (left_item is not (Tool tool and (UpgradeableShovel or ShovelTool)))
                {
                    return;
                }
                /// Check to see if Moon Misadventures is loaded, if it is, set upgrade level to 6, if not, set it to 5
                int maxToolUpgrade = ModEntry.MoonLoaded == true ? 6 : 5;
                if (tool.UpgradeLevel >= maxToolUpgrade)
                {
                    return;
                }
                ///Get the right item to upgrade the tool
                int upgradeItemIndex = tool.UpgradeLevel switch
                {
                    0 => ObjectIds.CopperBar,
                    1 => ObjectIds.IronBar,
                    2 => ObjectIds.GoldBar,
                    3 => ObjectIds.IridiumBar,
                    4 => ObjectIds.RadioactiveBar,
                    5 => "spacechase0.MoonMisadventures/Mythicite Bar".GetDeterministicHashCode(),
                    _ => ObjectIds.PrismaticShard,
                };
                ///If the right item is the ... right item, allow for tool upgrade
                if (right_item.ParentSheetIndex == upgradeItemIndex && right_item.Stack >= 5)
                {
                    ((UpgradeableShovel)left_item).UpgradeLevel++;
                }
                __result = left_item as UpgradeableShovel;
            }
        }
    }

    [HarmonyPatch(typeof(SpaceCore.Interface.NewForgeMenu), nameof(SpaceCore.Interface.NewForgeMenu.IsValidCraftIngredient))]
    class NewForgeMenuIsValidCraftIngredientPatcher
    {
        public static void Postfix(ref bool __result, Item item)
        {
            ///Make sure margo is loaded to do forge upgrading
            if (ModEntry.MargoLoaded)
            {
                ///If players have the Margo Compact turned off, do not do anything
                if (ModEntry.Config.MargoCompact == false)
                {
                    return;
                }
                ///Check to see if the item is a shovel or upgradeable shovel. If it is not, skip this code
                if (item is not (Tool tool and (UpgradeableShovel or ShovelTool)))
                {
                    return;
                }
                int maxToolUpgrade = ModEntry.MoonLoaded == true ? 7 : 6;
                if (tool.UpgradeLevel < maxToolUpgrade)
                {
                    __result = true;
                    Log.Warn(__result.ToString());
                }
            }
        }
    }

    [HarmonyPatch(typeof(SpaceCore.Interface.NewForgeMenu), nameof(SpaceCore.Interface.NewForgeMenu.IsValidCraft))]
    class NewForgeMenuIsValidCraftPatcher
    {
        public static void Postfix(ref bool __result, Item left_item, Item right_item)
        {
            ///If players have the Margo Compact turned off, do not do anything
            if (ModEntry.Config.MargoCompact == false)
            {
                return;
            }
            ///Check to see if the item is a shovel or upgradeable shovel. If it is not, skip this code
            if (left_item is not (Tool tool and (UpgradeableShovel or ShovelTool)))
            {
                return;
            }
            int maxToolUpgrade = ModEntry.MoonLoaded == true ? 6 : 5;
            if (tool.UpgradeLevel >= maxToolUpgrade)
            {
                return;
            }
            int upgradeItemIndex = tool.UpgradeLevel switch
            {
                0 => ObjectIds.CopperBar,
                1 => ObjectIds.IronBar,
                2 => ObjectIds.GoldBar,
                3 => ObjectIds.IridiumBar,
                4 => ObjectIds.RadioactiveBar,
                5 => "spacechase0.MoonMisadventures/Mythicite Bar".GetDeterministicHashCode(),
                _ => ObjectIds.PrismaticShard,
            };

            if (right_item.ParentSheetIndex == upgradeItemIndex && right_item.Stack >= 5)
            {
                __result = true;
            }
        }
    }

    [HarmonyPatch(typeof(Farmer), nameof(Farmer.showHoldingItem))]
    class Farmer_ShowHoldingItem
    {
        public static bool Prefix(
            Farmer who)
        {
            try
            {
                Item mrg = who.mostRecentlyGrabbedItem;
                if (mrg is UpgradeableShovel)
                {
                    Rectangle r = UpgradeableShovel.IconSourceRectangle((who.mostRecentlyGrabbedItem as Tool).UpgradeLevel);
                    switch (mrg)
                    {
                        case UpgradeableShovel:
                            r = UpgradeableShovel.IconSourceRectangle((who.mostRecentlyGrabbedItem as Tool).UpgradeLevel);
                            break;
                    }
                    Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(
                        textureName: ModEntry.Assets.ToolSpritesPath,
                        sourceRect: r,
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
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
            return true;
        }
    }


    ///Fix for sprite if using Margo
    [HarmonyPatch("DaLion.Overhaul.Modules.Tools.Patchers.Game1DrawToolPatcher", "ToolDrawInMenuPrefix")]
    class MARGO_Game1DrawToolPatcher
    {

        public static bool Prepare()
        {
            return ModEntry.MargoLoaded;
        }

        public static bool Prefix(Farmer f, ref (int, Texture2D)? __state)
        {
            var tool = f.CurrentTool;
            if (tool is UpgradeableShovel)
            {
                return false;
            }
            return true;
        }
    }
}
