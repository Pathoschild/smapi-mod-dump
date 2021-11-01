/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/AlternativeTextures
**
*************************************************/

using AlternativeTextures;
using AlternativeTextures.Framework.Models;
using AlternativeTextures.Framework.Patches.GameLocations;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace AlternativeTextures.Framework.Patches.SpecialObjects
{
    internal class WallpaperPatch : PatchTemplate
    {
        private readonly Type _object = typeof(Wallpaper);

        internal WallpaperPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(Wallpaper.placementAction), new[] { typeof(GameLocation), typeof(int), typeof(int), typeof(Farmer) }), postfix: new HarmonyMethod(GetType(), nameof(PlacementActionPostfix)));
        }

        private static void PlacementActionPostfix(Wallpaper __instance, ref bool __result, GameLocation location, int x, int y, Farmer who = null)
        {
            if (!__result)
            {
                return;
            }

            Point tile = new Point(x / 64, y / 64);
            DecoratableLocation decoratableLocation = who.currentLocation as DecoratableLocation;
            if ((bool)__instance.isFloor)
            {
                List<Rectangle> floors = decoratableLocation.getFloors();
                for (int j = 0; j < floors.Count; j++)
                {
                    if (floors[j].Contains(tile))
                    {
                        decoratableLocation.modData[$"AlternativeTexture.Floor.Owner_{j}"] = AlternativeTextures.DEFAULT_OWNER;
                        decoratableLocation.modData[$"AlternativeTexture.Floor.Name_{j}"] = String.Concat(AlternativeTextures.DEFAULT_OWNER, ".", $"{AlternativeTextureModel.TextureType.Decoration}_Floor_{Game1.GetSeasonForLocation(decoratableLocation)}");
                        decoratableLocation.modData[$"AlternativeTexture.Floor.Dirty_{j}"] = false.ToString();
                        decoratableLocation.modData[$"AlternativeTexture.Floor.Variation_{j}"] = "-1";
                        DecoratableLocationPatch.ResetFloorTiles(decoratableLocation, j, true);
                        return;
                    }
                }
            }
            else
            {
                List<Rectangle> walls = decoratableLocation.getWalls();
                for (int i = 0; i < walls.Count; i++)
                {
                    Rectangle wall = walls[i];
                    if (wall.Height == 2)
                    {
                        wall.Height = 3;
                    }
                    if (wall.Contains(tile))
                    {
                        decoratableLocation.modData[$"AlternativeTexture.Wallpaper.Owner_{i}"] = AlternativeTextures.DEFAULT_OWNER;
                        decoratableLocation.modData[$"AlternativeTexture.Wallpaper.Name_{i}"] = String.Concat(AlternativeTextures.DEFAULT_OWNER, ".", $"{AlternativeTextureModel.TextureType.Decoration}_Wallpaper_{Game1.GetSeasonForLocation(decoratableLocation)}");
                        decoratableLocation.modData[$"AlternativeTexture.Wallpaper.Dirty_{i}"] = false.ToString();
                        decoratableLocation.modData[$"AlternativeTexture.Wallpaper.Variation_{i}"] = "-1";
                        DecoratableLocationPatch.ResetWallTiles(decoratableLocation, i, true);
                        return;
                    }
                }
            }
        }
    }
}
