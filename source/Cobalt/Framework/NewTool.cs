using System.Collections.Generic;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace Cobalt.Framework
{
    internal class CobaltInjector : IAssetEditor
    {
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("TileSheets\\tools");
        }

        public void Edit<T>(IAssetData asset)
        {
            if ( asset.AssetNameEquals( "TileSheets\\tools" ) )
                asset.AsImage().PatchImage(ModEntry.instance.Helper.Content.Load<Texture2D>("assets/cobalt-tools.png"), null, null, PatchMode.Overlay);
        }
    }

    [HarmonyPatch(typeof(Utility), "priceForToolUpgradeLevel")]
    internal static class CobaltPriceHook
    {
        public static void Postfix(int level, ref int __result)
        {
            if (level == 5)
                __result = 100000;
        }
    }

    [HarmonyPatch(typeof(Utility), "getBlacksmithUpgradeStock")]
    internal static class CobaltUpgradeStockHook
    {
        public static void Postfix( StardewValley.Farmer who, Dictionary<Item, int[]> __result )
        {
            Tool tool = who.getToolFromName("Axe");
            if (tool != null && tool.UpgradeLevel == 4 )
            {
                var newTool = new Axe();
                newTool.UpgradeLevel = 5;
                __result.Add(newTool, new int[] { 100000, 1, CobaltBarItem.INDEX });
            }

            tool = who.getToolFromName("Watering Can");
            if (tool != null && tool.UpgradeLevel == 4)
            {
                var newTool = new WateringCan();
                newTool.UpgradeLevel = 5;
                __result.Add(newTool, new int[] { 100000, 1, CobaltBarItem.INDEX });
            }

            tool = who.getToolFromName("Pickaxe");
            if (tool != null && tool.UpgradeLevel == 4)
            {
                var newTool = new Pickaxe();
                newTool.UpgradeLevel = 5;
                __result.Add(newTool, new int[] { 100000, 1, CobaltBarItem.INDEX });
            }

            tool = who.getToolFromName("Hoe");
            if (tool != null && tool.UpgradeLevel == 4)
            {
                var newTool = new Hoe();
                newTool.UpgradeLevel = 5;
                __result.Add(newTool, new int[] { 100000, 1, CobaltBarItem.INDEX });
            }
        }
    }

    [HarmonyPatch(typeof(Tree), "performToolAction")]
    internal static class CobaltTreeFix
    {
        public static bool Prefix( Tree __instance, Tool t, int explosion )
        {
            if (t is Axe && t.UpgradeLevel == 5)
            {
                if (__instance.tapped.Value && explosion <= 0)
                    return true;
                if (__instance.growthStage.Value >= 5)
                    __instance.health.Value -= 10;
                else if (__instance.growthStage.Value >= 3)
                    __instance.health.Value -= 15;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(FruitTree), "performToolAction")]
    internal static class CobaltFruitTreeFix
    {
        public static bool Prefix(Tree __instance, Tool t, int explosion)
        {
            if (t is Axe && t.UpgradeLevel == 5)
            {
                if (__instance.tapped.Value && explosion <= 0)
                    return true;
                if (__instance.growthStage.Value >= 4)
                    __instance.health.Value -= 10f;
                else if (__instance.growthStage.Value >= 3)
                    __instance.health.Value -= 15f;
            }
            return true;
        }
    }
    
    [HarmonyPatch(typeof(SObject), "performToolAction")]
    internal static class CobaltObjectFix
    {
        public static bool Prefix(SObject __instance, Tool t)
        {
            if (t is Pickaxe && t.UpgradeLevel == 5)
            {
                if (__instance.name.Equals("Stone") && t.GetType() == typeof(Pickaxe))
                    if (__instance.ParentSheetIndex == 12 && t.UpgradeLevel == 1 || (__instance.ParentSheetIndex == 12 || __instance.ParentSheetIndex == 14) && t.UpgradeLevel == 0)
                    { }
                    else
                        __instance.MinutesUntilReady -= __instance.MinutesUntilReady;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Pickaxe), "DoFunction")]
    internal static class CobaltPickaxeFix
    {
        public static bool Prefix(Pickaxe __instance, GameLocation location, int x, int y, int power, Farmer who)
        {
            if (__instance.UpgradeLevel == 5)
            {
                int num1 = x / Game1.tileSize;
                int num2 = y / Game1.tileSize;
                Vector2 index = new Vector2((float)num1, (float)num2);
                location.Objects.TryGetValue(index, out SObject obj);
                if (obj == null)
                {
                    if (who.FacingDirection == 0 || who.FacingDirection == 2)
                    {
                        num1 = (x - 8) / Game1.tileSize;
                        location.Objects.TryGetValue(new Vector2((float)num1, (float)num2), out obj);
                        if (obj == null)
                        {
                            num1 = (x + 8) / Game1.tileSize;
                            location.Objects.TryGetValue(new Vector2((float)num1, (float)num2), out obj);
                        }
                    }
                    else
                    {
                        num2 = (y + 8) / Game1.tileSize;
                        location.Objects.TryGetValue(new Vector2((float)num1, (float)num2), out obj);
                        if (obj == null)
                        {
                            num2 = (y - 8) / Game1.tileSize;
                            location.Objects.TryGetValue(new Vector2((float)num1, (float)num2), out obj);
                        }
                    }
                    x = num1 * Game1.tileSize;
                    y = num2 * Game1.tileSize;
                }
                index = new Vector2((float)num1, (float)num2);
                if (obj != null)
                {
                    if (obj.Name.Equals("Stone"))
                    {
                        obj.MinutesUntilReady -= obj.MinutesUntilReady;
                    }
                }
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Quartz), "performToolAction")]
    internal static class CobaltQuartzFix
    {
        private static bool wasCobalt = false;

        public static bool Prefix(Quartz __instance, Tool t)
        {
            if (t is Pickaxe && t.UpgradeLevel == 5)
            {
                wasCobalt = true;
                t.UpgradeLevel = 4;
            }
            return true;
        }
        public static void Postfix(Quartz __instance, Tool t)
        {
            if ( wasCobalt )
            {
                t.UpgradeLevel = 5;
                wasCobalt = false;
            }
        }
    }
    
    /* Triggers the lovely harmony problems.
    [HarmonyPatch(typeof(WateringCan), "DoFunction")]
    static class CobaltWateringCanFix
    {
        public static void Postfix(WateringCan __instance)
        {
            if ( __instance.waterCanMax == __instance.WaterLeft && __instance.UpgradeLevel == 5 )
            {
                __instance.waterCanMax = __instance.WaterLeft = 1;
            }
        }
    }*/

    [HarmonyPatch(typeof(Tool), "get_DisplayName")]
    internal static class CobaltDisplayNameHook
    {
        public static bool Prefix( Tool __instance, ref string __result )
        {
            if ( __instance.UpgradeLevel == 5 )
            {
                __result = __instance.Name;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Tool), "get_Name")]
    internal static class CobaltNameHook
    {
        public static bool Prefix(Tool __instance, ref string __result)
        {
            if (__instance.UpgradeLevel == 5)
            {
                __result = "Cobalt " + __instance.Name;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Tool), "tilesAffected")]
    internal static class Tier5TilesHook
    {
        public static void Postfix(Tool __instance, List<Vector2> __result, Vector2 tileLocation, int power, StardewValley.Farmer who)
        {
            if ( power >= 6)
            {
                __result.Clear();
                switch ( who.FacingDirection )
                {
                    case 0:
                        for (int i = 0; i < 7 * 7; ++i)
                        {
                            int ix = i % 7 - 3;
                            int iy = i / 7 - 6;
                            __result.Add(tileLocation + new Vector2(ix, iy));
                        }
                        break;
                    case 1:
                        for (int i = 0; i < 7 * 7; ++i)
                        {
                            int ix = i % 7;
                            int iy = i / 7 - 3;
                            __result.Add(tileLocation + new Vector2(ix, iy));
                        }
                        break;
                    case 2:
                        for (int i = 0; i < 7 * 7; ++i)
                        {
                            int ix = i % 7 - 3;
                            int iy = i / 7;
                            __result.Add(tileLocation + new Vector2(ix, iy));
                        }
                        break;
                    case 3:
                        for (int i = 0; i < 7 * 7; ++i)
                        {
                            int ix = i % 7 - 6;
                            int iy = i / 7 - 3;
                            __result.Add(tileLocation + new Vector2(ix, iy));
                        }
                        break;
                }
            }
        }
    }
}
