/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NormanPCN/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using HarmonyLib;
using GenericModConfigMenu;
using StardewValley.Locations;
using xTile.Dimensions;
using xTile.Tiles;
using StardewModdingAPI.Events;

namespace FixFallTappedTrees
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {

        public static ModEntry Instance;

        public override void Entry(IModHelper helper)
        {
            Instance = this;

            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            helper.Events.GameLoop.DayStarted += DayStarted;

            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(Tree), nameof(Tree.seasonUpdate)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(seasonUpdate_Prefix)));

            harmony.Patch(
                original: AccessTools.Method(typeof(Tree), nameof(Tree.dayUpdate)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(dayUpdate_Postfix)));

        }

        private void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {

            // get Generic Mod Config Menu's API (if it's installed)
            var gmcm = Helper.ModRegistry.GetGenericModConfigMenuApi(this.Monitor);
            if (gmcm is null)
                return;
        }

        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            if (Game1.IsFall)
            {
                GameLocation location = Game1.getLocationFromName("Farm", false);
                if (location is not null)
                {
                    foreach (KeyValuePair<Vector2, TerrainFeature> pair in location.terrainFeatures.Pairs)
                    {
                        if (pair.Value is StardewValley.TerrainFeatures.Tree tree && tree.isTemporaryGreenRainTree.Value)
                        {
                            StardewValley.Object tile_object = location.getObjectAtTile((int)tree.Tile.X, (int)tree.Tile.Y);
                            if ((tile_object != null) && tile_object.IsTapper())
                            {
                                //in the game bug(?), tapped.Value has been set to false but the tapper remains on the tree.
                                //the tapper produce has not be altered...yet.
                                //the tree has been converted to a green rain tree by this point.
                                //assume a green rain tree with a tapper attached has been falsely converted from a normal tree,
                                Instance.Monitor.Log($"DayStarted: Changed tapped tree at ({tree.Tile.X}, {tree.Tile.Y}) back to normal.", LogLevel.Error);
                                if (tree.treeType.Value == "10")
                                {
                                    tree.treeType.Value = "1";
                                }
                                else
                                {
                                    tree.treeType.Value = "2";
                                }
                                tree.isTemporaryGreenRainTree.Value = false;
                                tree.tapped.Value = true;
                                tree.resetTexture();
                            }
                        }
                    }
                }
            }
        }

        public static bool seasonUpdate_Prefix(Tree __instance, bool onLoad)
        {
            //bool logic from game code.
            if (
                Game1.IsFall &&
                !__instance.tapped &&
                (__instance.treeType.Value == "1" || __instance.treeType.Value == "2") &&
                __instance.growthStage.Value >= 5 &&
                __instance.Location != null &&
                !(__instance.Location is Town) && !__instance.Location.IsGreenhouse
               )
            {
                StardewValley.Object tile_object = __instance.Location.getObjectAtTile((int)__instance.Tile.X, (int)__instance.Tile.Y);
                if (tile_object != null && tile_object.IsTapper())
                {
                    Instance.Monitor.Log($"seasonUpdate: Tree !tapped with a tapper attached. ({__instance.Tile.X}, {__instance.Tile.Y}) onLoad={onLoad} tapped={__instance.tapped.Value} hasMoss={__instance.hasMoss.Value}.", LogLevel.Error);
                    __instance.tapped.Value = true;
                }
            }
            return true;
        }

        public static void dayUpdate_Postfix(Tree __instance)
        {
            if (
                Game1.IsFall &&
                !__instance.tapped.Value &&
                (__instance.treeType.Value == "1" || __instance.treeType.Value == "2") &&
                __instance.growthStage.Value >= 5 &&
                __instance.Location != null &&
                !(__instance.Location is Town) && !__instance.Location.IsGreenhouse
               )
            {
                StardewValley.Object tile_object = __instance.Location.getObjectAtTile((int)__instance.Tile.X, (int)__instance.Tile.Y);
                if (tile_object is not null && tile_object.IsTapper())
                {
                    Instance.Monitor.Log($"dayUpdate: Tree !tapped with a tapper attached. hasMoss={__instance.hasMoss.Value} ({__instance.Tile.X}, {__instance.Tile.Y})", LogLevel.Error);
                    __instance.tapped.Value = true;
                }
            }
        }
    }
}