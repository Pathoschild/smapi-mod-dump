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
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace AlternativeTextures.Framework.Patches.GameLocations
{
    internal class GameLocationPatch : PatchTemplate
    {
        private readonly Type _object = typeof(GameLocation);

        internal GameLocationPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(GameLocation.checkAction), new[] { typeof(xTile.Dimensions.Location), typeof(xTile.Dimensions.Rectangle), typeof(Farmer) }), prefix: new HarmonyMethod(GetType(), nameof(CheckActionPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(GameLocation.LowPriorityLeftClick), new[] { typeof(int), typeof(int), typeof(Farmer) }), prefix: new HarmonyMethod(GetType(), nameof(LowPriorityLeftClickPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(GameLocation.leftClick), new[] { typeof(int), typeof(int), typeof(Farmer) }), prefix: new HarmonyMethod(GetType(), nameof(LowPriorityLeftClickPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(GameLocation.seasonUpdate), new[] { typeof(string), typeof(bool) }), postfix: new HarmonyMethod(GetType(), nameof(SeasonUpdatePostfix)));
        }

        private static bool CheckActionPrefix(GameLocation __instance, ref bool __result, xTile.Dimensions.Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
        {
            if (Game1.didPlayerJustRightClick())
            {
                return true;
            }

            if (who.CurrentTool is GenericTool tool && (tool.modData.ContainsKey(AlternativeTextures.PAINT_BUCKET_FLAG) || tool.modData.ContainsKey(AlternativeTextures.PAINT_BRUSH_FLAG)))
            {
                Vector2 position = ((!Game1.wasMouseVisibleThisFrame) ? Game1.player.GetToolLocation() : new Vector2(Game1.getOldMouseX() + Game1.viewport.X, Game1.getOldMouseY() + Game1.viewport.Y));
                tool.beginUsing(__instance, (int)position.X, (int)position.Y, who);
                __result = false;
                return false;
            }

            return true;
        }

        private static bool LowPriorityLeftClickPrefix(GameLocation __instance, ref bool __result, int x, int y, Farmer who)
        {
            if (who.CurrentTool is GenericTool tool && (tool.modData.ContainsKey(AlternativeTextures.PAINT_BUCKET_FLAG) || tool.modData.ContainsKey(AlternativeTextures.PAINT_BRUSH_FLAG)))
            {
                __result = false;
                return false;
            }

            return true;
        }

        internal static void SeasonUpdatePostfix(GameLocation __instance, string season, bool onLoad = false)
        {
            if (__instance is null)
            {
                return;
            }

            if (__instance.objects != null)
            {
                for (int k = __instance.objects.Count() - 1; k >= 0; k--)
                {
                    var obj = __instance.objects.Pairs.ElementAt(k).Value;
                    if (obj.modData.ContainsKey("AlternativeTextureSeason") && !String.IsNullOrEmpty(obj.modData["AlternativeTextureSeason"]) && !String.Equals(obj.modData["AlternativeTextureSeason"], Game1.currentSeason, StringComparison.OrdinalIgnoreCase))
                    {
                        var instanceName = GetObjectName(obj);
                        if (obj is Fence fence && fence.isGate)
                        {
                            instanceName = Game1.objectInformation[325].Split('/')[0];
                        }
                        obj.modData["AlternativeTextureSeason"] = Game1.currentSeason;
                        obj.modData["AlternativeTextureName"] = String.Concat(obj.modData["AlternativeTextureOwner"], ".", $"{AlternativeTextureModel.TextureType.Craftable}_{instanceName}_{obj.modData["AlternativeTextureSeason"]}");
                    }
                }
            }

            if (__instance.characters != null)
            {
                for (int k = __instance.characters.Count() - 1; k >= 0; k--)
                {
                    var character = __instance.characters.ElementAt(k);
                    if (character.modData.ContainsKey("AlternativeTextureSeason") && !String.IsNullOrEmpty(character.modData["AlternativeTextureSeason"]) && !String.Equals(character.modData["AlternativeTextureSeason"], Game1.currentSeason, StringComparison.OrdinalIgnoreCase))
                    {
                        var instanceName = GetCharacterName(character);
                        character.modData["AlternativeTextureSeason"] = Game1.currentSeason;
                        character.modData["AlternativeTextureName"] = String.Concat(character.modData["AlternativeTextureOwner"], ".", $"{AlternativeTextureModel.TextureType.Character}_{instanceName}_{character.modData["AlternativeTextureSeason"]}");
                    }
                }
            }

            // Check for animals, if __instance is an applicable location
            if ((__instance is Farm farm && farm.animals != null) || (__instance is AnimalHouse animalHouse && animalHouse.animals != null))
            {
                var animals = __instance is Farm ? (__instance as Farm).animals.Values : (__instance as AnimalHouse).animals.Values;
                for (int k = animals.Count() - 1; k >= 0; k--)
                {
                    var farmAnimal = animals.ElementAt(k);
                    if (farmAnimal.modData.ContainsKey("AlternativeTextureSeason") && !String.IsNullOrEmpty(farmAnimal.modData["AlternativeTextureSeason"]) && !String.Equals(farmAnimal.modData["AlternativeTextureSeason"], Game1.currentSeason, StringComparison.OrdinalIgnoreCase))
                    {
                        var instanceName = GetCharacterName(farmAnimal);
                        farmAnimal.modData["AlternativeTextureSeason"] = Game1.currentSeason;
                        farmAnimal.modData["AlternativeTextureName"] = String.Concat(farmAnimal.modData["AlternativeTextureOwner"], ".", $"{AlternativeTextureModel.TextureType.Character}_{instanceName}_{farmAnimal.modData["AlternativeTextureSeason"]}");
                    }
                }
            }
        }
    }
}